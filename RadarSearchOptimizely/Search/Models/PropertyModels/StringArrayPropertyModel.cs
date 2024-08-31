using EPiServer.Core;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class StringArrayPropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public StringArrayPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;
            if (property == null || property.Value == null)
                return;
            var prop = property.Value as string[];
            if (prop != null)
            {
                foreach (var s in prop)
                {
                    _searchValue += s + " ";
                }
            }

            _searchValue = SpecialTags.StartReplaceTag + " " + _searchValue.Trim().HtmlStrip() + " " + SpecialTags.EndReplaceTag;
        }

        public override string SearchValue()
        {
            return " " + _searchValue + " ";
        }
    }
}
