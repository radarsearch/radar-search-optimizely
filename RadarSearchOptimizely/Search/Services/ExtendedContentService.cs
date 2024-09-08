using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.Web.Routing;

namespace RadarSearchOptimizely.Search.Services
{
    public interface IExtendedContentRepository
    {
        IEnumerable<TContent> GetAllContent<TContent>(ContentReference root, string language) where TContent : IContent;
        IContentRepository Base { get; }
        string GetRelativeUrl(IContent content, string language);
    }

    public class ExtendedContentService : IExtendedContentRepository
    {
        public IContentRepository Base { get; }
        private readonly ILanguageBranchRepository _languageRepository;
        private readonly EPiServer.Logging.ILogger _logger;
        private readonly UrlResolver _urlResolver;

        public ExtendedContentService(IContentRepository contentRepository, ILanguageBranchRepository languageRepository, UrlResolver urlResolver)
        {
            Base = contentRepository;
            _languageRepository = languageRepository;
            _logger = LogManager.GetLogger(typeof(ExtendedContentService));
            _urlResolver = urlResolver;
        }

        public IEnumerable<TContent> GetAllContent<TContent>(ContentReference root, string language) where TContent : IContent
        {
            var content = GetAllContent(root, language).OfType<TContent>().ToList();

            _logger.Debug("ExtendedContentRepository: Fetched all content from database. Amount of items fetched: {0}", content.Count);

            return content;
        }

        public string GetRelativeUrl(IContent content, string language)
        {
            var virtualPathProvider = (language == "global" || string.IsNullOrEmpty(language)) ?
                _urlResolver.GetVirtualPath(content.ContentLink) :
                _urlResolver.GetVirtualPath(content.ContentLink, language);
            return virtualPathProvider != null ? virtualPathProvider.VirtualPath : string.Empty;
        }

        private IEnumerable<IContent> GetAllContent(ContentReference root, string language)
        {
            var allContent = Base.GetDescendents(root);

            var allLanguages = _languageRepository.ListEnabled();

            foreach (var contentReference in allContent)
            {
                IContent currentContent;

                if (!string.IsNullOrEmpty(language))
                {
                    if (Base.TryGet(contentReference, new CultureInfo(language), out currentContent))
                        yield return currentContent;
                    continue;
                }

                var currentContentResult = Base.TryGet(contentReference, out currentContent);

                if (currentContentResult && (currentContent is PageData || currentContent is BlockData))
                {
                    foreach (var languageBranch in allLanguages)
                    {
                        IContent content;
                        var result = Base.TryGet(contentReference, new LanguageSelector(languageBranch.LanguageID), out content);
                        if (result)
                        {
                            yield return content;
                        }
                    }
                    continue;
                }
                yield return currentContent;
            }
        }
    }
}