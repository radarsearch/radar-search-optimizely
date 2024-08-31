using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;

namespace RadarSearchOptimizely.Search.Extensions
{

    public static class ContentExtensions
    {
        public static IEnumerable<T> FilterForDisplay<T>(this IEnumerable<T> contents, bool requirePageTemplate = false, bool requireVisibleInMenu = false)
            where T : IContent
        {
            var accessFilter = new FilterAccess();
            var publishedFilter = new FilterPublished();
            contents = contents.Where(x => !publishedFilter.ShouldFilter(x) && !accessFilter.ShouldFilter(x));
            if (requirePageTemplate)
            {
                var templateFilter = ServiceLocator.Current.GetInstance<FilterTemplate>();
                templateFilter.TemplateTypeCategories = TemplateTypeCategories.Request;
                contents = contents.Where(x => !templateFilter.ShouldFilter(x));
            }
            if (requireVisibleInMenu)
            {
                contents = contents.Where(x => VisibleInMenu(x));
            }
            return contents;
        }

        private static bool VisibleInMenu(IContent content)
        {
            return content is not PageData page || page.VisibleInMenu;
        }

        public static bool ShouldBeVisible(this IContent content)
        {
            if (content.IsDeleted)
                return false;
            if (content is not IVersionable versionable)
                return false;

            if (versionable.IsPendingPublish)
                return false;
            if (versionable.Status is VersionStatus.DelayedPublish or not VersionStatus.Published)
                return false;

            if (versionable.StopPublish == null)
                return true;

            var stopPublish = versionable.StopPublish.GetValueOrDefault(DateTime.MaxValue);
            return stopPublish > DateTime.Now;
        }

        public static string FindContentType(this IContent content)
        {
            if (content == null)
                return null;

            var fromContentRepository = content.GetContentTypeFromContentRepository();
            var currentType = content.GetType();
            var concreteTypes = currentType.GetParentTypes().Where(x => !x.IsInterface).ToList();

            if (concreteTypes.Count < 1)
                return null;

            var interfaces = currentType.GetParentTypes().Where(x => x.IsInterface).ToList();

            var types = concreteTypes.Select(concreteType => concreteType.Name).ToList();
            if (fromContentRepository != null && !types.Any(x => x.Equals(fromContentRepository)))
                types.Add(fromContentRepository);

            return types.Aggregate(string.Empty, (current, type) => current + (type + " ")).Trim(' ');
        }

        public static string GetContentTypeFromContentRepository(this IContent content)
        {
            if (content == null)
                return null;
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var list = contentTypeRepository?.List();
            if (list == null)
                return null;
            var contentTypes = list as ContentType[] ?? list.ToArray();
            var ct = !contentTypes.Any() ? null : contentTypes.FirstOrDefault(contentType => content.ContentTypeID == contentType.ID);
            return ct != null ? ct.Name : null;
        }

        public static void ResolveModifyData(this object content, IRadarIndexData radarIndexData)
        {
            try
            {
                if (content is not IRadarSearchModifyData radarSearchModifyData) return;

                radarSearchModifyData.ConvertModifyData(radarIndexData);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}