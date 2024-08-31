using System.Globalization;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;

namespace RadarSearchOptimizely.Search.EventHandler
{
    [InitializableModule]
    [ModuleDependency(typeof(global::EPiServer.Web.InitializationModule))]
    public class RadarSearchEventHandler : IInitializableModule
    {
        private IContentLoader _contentLoader;
        private ISearchService _searchService;
        private IRadarContentService _radarContentService;

        private static readonly object IndexLock = new object();

        public void Initialize(InitializationEngine context)
        {
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            _searchService = ServiceLocator.Current.GetInstance<ISearchService>();
            _radarContentService = ServiceLocator.Current.GetInstance<IRadarContentService>();

            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.PublishedContent += EventsContent;
            events.DeletedContent += EventsContent;
            events.MovedContent += EventsContent;
        }

        private void EventsContent(object sender, ContentEventArgs e)
        {
            var logger = LogManager.GetLogger(typeof(RadarSearchEventHandler));

            lock (IndexLock)
            {
                try
                {
                    Task.Run(async () =>
                    {
                        // Delay to avoid deadlock with other event-hooks
                        await Task.Delay(1000);

                        var content = _contentLoader.Get<IContent>(e.ContentLink);

                        logger.Debug("Indexing {0}.", content.Name);

                        var indexData = await _radarContentService.ParseData(content);

                        logger.Debug("Finished indexing {0}.", content.Name);
                        logger.Debug("Getting content in content areas {0}.", content.Name);

                        var contentToUpdate = _radarContentService.GetContentInContentAreaWithContentReference(content.ContentLink)?.ToList();

                        logger.Debug("Finished Getting content in content areas with content: {0}. Number is: {1}", content.Name, contentToUpdate?.Count);

                        var descendents = _contentLoader.GetDescendents(content.ContentLink);
                        var descendentsContent = _contentLoader.GetItems(descendents, new CultureInfo(content.LanguageBranch() ?? CultureInfo.InvariantCulture.Name)).ToList();

                        logger.Debug("Finished getting children of {0}. Number is: {1}", content.Name, descendentsContent.Count);

                        if (contentToUpdate?.Any() == true)
                        {
                            var indexDataToUpdate = await _radarContentService.ParseData(contentToUpdate);
                            indexData = indexData.Concat(indexDataToUpdate).ToList();
                        }
                        if (descendentsContent.Any())
                        {
                            var indexDataDescendents = await _radarContentService.ParseData(descendentsContent);
                            indexData = indexData.Concat(indexDataDescendents).ToList();
                        }

                        var indexDataMessage = await _searchService.SendIndexData(indexData);
                        logger.Debug(indexDataMessage);
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }


        public void Uninitialize(InitializationEngine context)
        {
            _contentLoader = null;
            _searchService = null;
            _radarContentService = null;

            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.PublishedContent -= EventsContent;
            events.DeletedContent -= EventsContent;
            events.MovedContent -= EventsContent;
        }
    }
}
