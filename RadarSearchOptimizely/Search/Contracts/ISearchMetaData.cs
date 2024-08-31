using RadarSearchOptimizely.Search.Models.Enums;
using Index = RadarSearchOptimizely.Search.Models.Enums.Index;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface ISearchMetaData
    {
        string FieldName { get; set; }
        string Value { get; set; }
        float Boost { get; set; }
        Index FieldIndex { get; set; }
        Store FieldStore { get; set; }
        int ExtremeBoost { get; set; }
    }
}
