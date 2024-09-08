using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IContentIndexModel
    {
        IContent Content { get; set; }
        IEnumerable<IPropertyModel> Properties { get; set; }
    }
}