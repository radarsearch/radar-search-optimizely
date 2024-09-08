using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IPropertyIndexService
    {
        IEnumerable<IPropertyModel> GetAllProperties(IContent content);
    }
}