using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class PageTypePropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public PageTypePropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;
        }

        public override string SearchValue()
        {
            return _searchValue;
        }
    }
}
