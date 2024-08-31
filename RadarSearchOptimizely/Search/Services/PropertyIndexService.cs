using EPiServer.Core;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Services
{
    public class PropertyIndexService : IPropertyIndexService
    {
        public IEnumerable<IPropertyModel> GetAllProperties(IContent content)
        {
            return from property in content.Property where property.IsPropertyData select property.CreatePropertyModel(content, true, false);
        }
    }
}
