using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class ContentAreaExtensions
    {
        public static IEnumerable<BlockData> FindBlocks<TBlockData>(this ContentArea contentArea) where TBlockData : BlockData
        {
            if (contentArea == null || contentArea.Count <= 0 || contentArea.Items.Count <= 0)
                return new List<TBlockData>();
            var items = contentArea.Items;
            var blocks = new List<TBlockData>();
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            foreach (var contentAreaItem in items)
            {
                TBlockData content;
                var result = repository.TryGet(contentAreaItem.ContentLink,
                    new LanguageSelector(ContentLanguage.PreferredCulture.Name), out content);
                if (result)
                    blocks.Add(content);
            }
            return blocks;
        }

        public static IContent FindFirstContent(this ContentArea area)
        {
            if (area == null || area.Count <= 0 || area.Items.Count <= 0) return null;
            var item = area.Items.First();
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var content = repo.Get<IContent>(item.ContentLink);
            return content;
        }

        public static IContent FindContentOfType<TContent>(this ContentArea area) where TContent : IContent
        {
            if (area == null || area.Count <= 0 || area.Items.Count <= 0) return null;
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = area.Items;
            foreach (var item in items)
            {
                IContent current;
                if (repo.TryGet(item.ContentLink, out current) && current is TContent)
                    return current;
            }
            return null;
        }

        public static IEnumerable<IContent> FindAllContent(this ContentArea area)
        {
            if (area == null || area.Count <= 0 || area.Items.Count <= 0) yield break;
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = area.Items;
            foreach (var item in items)
            {
                IContent current;
                if (repo.TryGet(item.ContentLink, out current))
                    yield return current;
            }
        }

        public static IEnumerable<TContent> FindAllContentOfType<TContent>(this ContentArea area) where TContent : IContent
        {
            if (area == null || area.Count <= 0 || area.Items.Count <= 0) yield break;
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = area.Items;
            foreach (var item in items)
            {
                TContent current;
                if (repo.TryGet(item.ContentLink, out current))
                    yield return current;
            }
        }

        public static IEnumerable<TContent> FindAllContentOfType<TContent>(this ContentArea area, CultureInfo currentCulture) where TContent : IContent
        {
            if (area == null || area.Count <= 0 || area.Items.Count <= 0) yield break;
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var items = area.Items;
            foreach (var item in items)
            {
                TContent current;
                if (repo.TryGet(item.ContentLink, currentCulture, out current))
                    yield return current;
            }
        }
    }
}
