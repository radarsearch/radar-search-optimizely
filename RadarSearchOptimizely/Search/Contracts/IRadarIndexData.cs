using RadarSearchOptimizely.Search.Models;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IRadarIndexData
    {
        string Id { get; }
        int ContentId { get; set; }
        Guid Guid { get; }
        string Unique { get; }
        string Language { get; }
        bool Deleted { get; set; }
        bool HideInIndex { get; set; }
        string Url { get; set; }
        string RelativeUrl { get; set; }
        List<CategoryData> Categories { get; set; }
        string Type { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Text { get; set; }
        List<SearchMetaData> Metadata { get; set; }
        string Image { get; set; }
        DateTime Updated { get; set; }
        DateTime Created { get; set; }
        List<AccessControlEntry> Acl { get; set; }
        string AclString { get; set; }
        IndexStatus IndexStaus { get; set; }
        void SetId(Guid guid);
        void SetId(string unique);
        void SetLanguage(string language);
        string[] Keywords { get; set; }
        string GetAccessControlEntriesAsString();
        string GetKeywordsAsIndexString();
    }
}
