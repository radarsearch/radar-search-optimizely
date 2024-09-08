using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IRadarContentService
    {
        Task<List<IRadarIndexData>> ParseAllData();
        Task<List<IRadarIndexData>> ParseData(IContent contentToIndex);
        Task<List<IRadarIndexData>> ParseData(List<IContent> contentToIndex);
        IContentIndexModel ResolveContentIndexModel(IContent content);
        IRadarIndexData ResolveSearchData(IContentIndexModel contentIndexModel);
        Task<IEnumerable<IRadarIndexData>> GetDataDeletedInEpiButNotInSearchIndex(List<IContent> episerverData);
        IEnumerable<IContent> GetContentInContentAreaWithContentReference(ContentReference contentReference);
    }
}