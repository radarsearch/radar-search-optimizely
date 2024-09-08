using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IAccessControlEntry
    {
        string Name { get; set; }
        AclLevel Level { get; set; }
    }
}