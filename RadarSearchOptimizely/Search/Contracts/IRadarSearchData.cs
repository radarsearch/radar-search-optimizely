using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IRadarSearchData
    {
        string Id { get; }
        Guid Guid { get; }
        string Unique { get; }
        string Language { get; }

        string Text { get; }
        DateTime Updated { get; }
        DateTime Created { get; }
        string Url { get; }
        string RelativeUrl { get; }
        string Type { get; }
        List<AccessControlEntry> Acl { get; }

        string Title { get; }
        string Description { get; }
        string Image { get; }
        List<CategoryData> Categories { get; set; }
        List<SearchMetaData> Metadata { get; }
        bool Delete { get; }
        bool FileContent { get; }
        string[] Keywords { get; }
    }
}