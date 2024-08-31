using EPiServer.Core;
using RadarSearchOptimizely.Search.Contracts;

namespace RadarSearchOptimizely.Search.Models
{
    public class ContentIndexModel : IContentIndexModel
    {
        public IContent Content { get; set; }
        public IEnumerable<IPropertyModel> Properties { get; set; }
    }
}
