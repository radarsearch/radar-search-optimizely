using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models;
using RadarSearchOptimizely.Search.Models.Enums;
using AccessControlEntry = RadarSearchOptimizely.Search.Models.AccessControlEntry;

namespace RadarSearchOptimizely.Search.Services
{
    public class RadarContentService : IRadarContentService
    {
        private readonly IExtendedContentRepository _contentRepository;
        private readonly ISearchService _searchService;
        private readonly IPropertyIndexService _propertyIndexService;
        private readonly IConfiguration _configuration;
        private readonly EPiServer.Logging.ILogger _logger;

        public RadarContentService(IExtendedContentRepository contentRepository,
            ISearchService searchService,
            IPropertyIndexService propertyIndexService,
            IConfiguration configuration)
        {
            _contentRepository = contentRepository;
            _searchService = searchService;
            _propertyIndexService = propertyIndexService;
            _configuration = configuration;
            _logger = LogManager.GetLogger(typeof(RadarContentService));
        }

        public async Task<List<IRadarIndexData>> ParseAllData()
        {
            var allContentToIndex = _contentRepository.GetAllContent<IContent>(ContentReference.RootPage, string.Empty).ToList();
            LogManager.GetLogger(typeof(IRadarContentService)).Information($"Parsing {allContentToIndex.Count} pages");

            var dataToIndex = await ParseData(allContentToIndex);

            var allDataToCheck = _contentRepository.GetAllContent<IContent>(ContentReference.RootPage, string.Empty).ToList();
            var dataDeletedInEpiNotInSearchIndex = (await GetDataDeletedInEpiButNotInSearchIndex(allDataToCheck)).ToList();

            dataToIndex.AddRange(dataDeletedInEpiNotInSearchIndex);

            return dataToIndex;
        }

        public async Task<List<IRadarIndexData>> ParseData(List<IContent> contentToIndex)
        {
            var contentIndexModels = contentToIndex.Select(ResolveContentIndexModel).ToList();
            var dataToIndex = contentIndexModels.Select(ResolveSearchData).ToList();

            LogManager.GetLogger(typeof(IRadarContentService)).Information($"{dataToIndex.Count} pages ready for indexing");

            return await Task.FromResult(dataToIndex);
        }

        public async Task<List<IRadarIndexData>> ParseData(IContent contentToIndex)
        {
            List<IRadarIndexData> indexData;

            indexData = await ParseData(new List<IContent> { contentToIndex });

            return indexData;
        }

        public async Task<IEnumerable<IRadarIndexData>> GetDataDeletedInEpiButNotInSearchIndex(List<IContent> episerverData)
        {
            var allInIndex = await _searchService.GetAllIndexData();
            var notInEpiserver = allInIndex
                .Where(d => !episerverData
                    .Any(e => e.ContentGuid.ToString().Equals(d.Guid.ToString(), StringComparison.OrdinalIgnoreCase)))
                .ToList();

            _logger.Debug("RADAR : GetDataDeletedInEpiButNotInSearchIndex - Amount of pages not in Episerver is: {0}", notInEpiserver.Count);

            notInEpiserver.ForEach(x => x.Deleted = true);

            return notInEpiserver;
        }

        public IContentIndexModel ResolveContentIndexModel(IContent content)
        {
            var model = new ContentIndexModel
            {
                Content = content,
                Properties = _propertyIndexService.GetAllProperties(content)
            };
            return model;
        }

        public IRadarIndexData ResolveSearchData(IContentIndexModel contentIndexModel)
        {
            var searchData = new RadarIndexData() { ContentId = contentIndexModel.Content.ContentLink.ID };
            try
            {
                var language = string.IsNullOrEmpty(contentIndexModel.Content.LanguageBranch()) ? Const.GlobalLanguage : contentIndexModel.Content.LanguageBranch();
                var versionable = contentIndexModel.Content as IVersionable;

                if (contentIndexModel.Content.ContentGuid == Guid.Empty || string.IsNullOrEmpty(language))
                {
                    return null;
                }

                searchData.SetId(contentIndexModel.Content.ContentGuid);
                searchData.SetLanguage(language);
                searchData.Title = contentIndexModel.Content.Name;
                searchData.Deleted = contentIndexModel.Content.IsDeleted;
                searchData.Type = contentIndexModel.Content.FindContentType();
                searchData.Description = contentIndexModel.Content.Property["MainBody"] != null
                    ? (contentIndexModel.Content.Property["MainBody"].Value as string).HtmlStrip()
                    : string.Empty;

                if (searchData.Deleted)
                    return searchData;

                searchData.Deleted = !contentIndexModel.Content.ShouldBeVisible();

                if (versionable != null)
                    searchData.Created = versionable.StartPublish ?? searchData.Created;

                if (searchData.Deleted)
                    return searchData;

                if (contentIndexModel.Content is PageData pd)
                {
                    var categories = pd.Category;
                    if (categories != null)
                    {
                        var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
                        foreach (var catid in categories)
                        {
                            var category = categoryRepository.Get(catid);
                            if (category != null)
                            {
                                searchData.Categories.Add(new CategoryData
                                {
                                    Id = category.ID,
                                    Name = category.Name,
                                    Description = category.Description
                                });
                            }
                        }
                    }
                }

                try
                {
                    searchData.Url = SiteDefinition.Current.Hosts.FirstOrDefault(h => h.Type == HostDefinitionType.Primary && !h.IsWildcardHost())?.Url.ToString();
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger(typeof(IRadarContentService)).Error($"Error resolving site url: {ex.Message}", ex);
                }

                searchData.RelativeUrl = _contentRepository.GetRelativeUrl(contentIndexModel.Content, language);

                searchData.Acl = new List<AccessControlEntry>();

                var roles = contentIndexModel.Content.ToRawACEArray();
                foreach (var rawAce in roles)
                {
                    searchData.Acl.Add(new AccessControlEntry
                    {
                        Name = rawAce.Name,
                        Level = (AclLevel)rawAce.Access
                    });
                }

                var properties = contentIndexModel.Properties.Where(x => x.Searchable).ToList();

                foreach (var propertyModel in properties)
                {
                    searchData.Text += propertyModel.SearchValue() + " ";
                }

                searchData.Text = string.IsNullOrEmpty(searchData.Text) ? string.Empty : searchData.Text.RemoveDuplcateSpaces();

                contentIndexModel.Content.ResolveModifyData(searchData);

                var settings = _configuration.GetSection("RadarSearchConfiguration");
                var types = settings.GetSection("IgnoreContentTypes").Value.Split(',');
                var indexedTypes = searchData.Type.Split(' ');

                foreach (var type in types)
                {
                    if (!indexedTypes.Contains(type))
                        continue;
                    searchData.HideInIndex = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(IRadarContentService)).Error($"Error resolving search data: {ex.Message}", ex);
                searchData.Deleted = true;
            }
            return searchData;
        }

        public IEnumerable<IContent> GetContentInContentAreaWithContentReference(ContentReference contentReference)
        {
            var references = _contentRepository.Base.GetReferencesToContent(contentReference, false);

            foreach (var reference in references)
            {
                var selector = new LanguageSelector(reference.OwnerLanguage.Name);
                if (!_contentRepository.Base.TryGet<PageData>(reference.OwnerID, selector, out var content)) continue;
                var found = false;

                foreach (var propertyData in content.Property)
                {
                    if (found)
                        break;

                    var propertyType = propertyData.PropertyValueType;

                    switch (propertyData.IsPropertyData)
                    {
                        case true when propertyType == typeof(ContentArea):
                            {
                                if (propertyData.Value is not ContentArea area)
                                    continue;

                                foreach (var contentAreaItem in area.Items)
                                {
                                    if (found)
                                        break;

                                    if (!contentAreaItem.ContentLink.ID.Equals(contentReference.ID))
                                        continue;

                                    found = true;
                                    yield return content;
                                }

                                break;
                            }
                        case true when propertyType == typeof(XhtmlString):
                            {
                                if (propertyData.Value is not XhtmlString xhtmlString)
                                    continue;

                                var fragments = xhtmlString.Fragments.Where(x => x is ContentFragment).ToList();

                                if (fragments.OfType<ContentFragment>().All(fragment => fragment.ContentLink.ID != contentReference.ID))
                                    continue;

                                found = true;
                                yield return content;

                                break;
                            }
                    }
                }
            }
        }
    }
}
