using System.Runtime.Serialization;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models
{
    [Serializable]
    [DataContract]
    public class AccessControlEntry : IAccessControlEntry
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public AclLevel Level { get; set; }
    }
}
