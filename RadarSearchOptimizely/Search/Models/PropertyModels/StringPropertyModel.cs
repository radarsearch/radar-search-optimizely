using EPiServer.Core;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class StringPropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public StringPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;
            if (property == null || property.Value == null)
                return;
            _searchValue = property.Value.ToString();
        }

        public override string SearchValue()
        {
            return " " + _searchValue.HtmlStrip() + " ";
        }
    }
}