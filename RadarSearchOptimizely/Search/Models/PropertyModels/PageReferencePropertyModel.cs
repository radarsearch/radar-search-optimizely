using System.Globalization;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;
using RadarSearchOptimizely.Search.Services;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class PageReferencePropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public PageReferencePropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;

            try
            {
                if (property == null || property.Value == null)
                    return;
                var contentReference = property.Value as ContentReference;
                if (contentReference == null)
                    return;

                var contentRepository = ServiceLocator.Current.GetInstance<IExtendedContentRepository>();
                var lang = content.LanguageBranch();

                PageData pageData;
                bool result;

                if (string.IsNullOrEmpty(lang) || lang == Const.GlobalLanguage)
                    result = contentRepository.Base.TryGet(contentReference, out pageData);
                else
                    result = contentRepository.Base.TryGet(contentReference, new CultureInfo(lang), out pageData);

                if (!result)
                    return;

                var url = contentRepository.GetRelativeUrl(pageData, lang);
                var text = pageData.Name;

                var radardata = pageData as IRadarSearchModifyData;

                if (radardata != null)
                {
                    text = string.IsNullOrEmpty(radardata.SearchData().Title) ? radardata.SearchData().Title : pageData.Name;
                }
                _searchValue = SpecialTags.StartReplaceTag + " " + text.Trim().HtmlStrip() + " " + SpecialTags.EndReplaceTag;
            }
            catch (Exception)
            {
            }
        }

        public override string SearchValue()
        {
            return " " + _searchValue + " ";
        }
    }
}