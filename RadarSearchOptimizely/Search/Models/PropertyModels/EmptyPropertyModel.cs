using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class EmptyPropertyModel : PropertyModel
    {
        private readonly string _searchData;

        public EmptyPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchData = string.Empty;
        }

        public override string SearchValue()
        {
            return _searchData;
        }
    }
}