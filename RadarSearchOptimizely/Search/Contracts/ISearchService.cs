using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface ISearchService
    {
        [Obsolete("This method is deprecated. Use Search(RadarQuery)", false)]
        Task<SearchResult> Search(SearchResult searchResult);
        /**
         * Executes a search with the given query. 
         */
        Task<SearchResult> Search(RadarQuery query);
        Task<string> SendIndexData(List<IRadarIndexData> indexData);
        Task<List<RadarIndexData>> GetAllIndexData();
        Task<string> RunQueue();
    }
}