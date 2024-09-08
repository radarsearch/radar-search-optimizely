using EPiServer.Core;
using EPiServer.SpecializedProperties;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class LinkCollectionPropertyModel : PropertyModel
    {
        private readonly string _searchData;

        public LinkCollectionPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchData = string.Empty;
            if (property == null || property.Value == null)
                return;
            var collection = property.Value as LinkItemCollection;
            if (collection == null)
                return;
            foreach (var c in collection)
            {
                _searchData += c.Text + " ";
            }
            _searchData = SpecialTags.StartReplaceTag + " " + _searchData.Trim().HtmlStrip() + " " + SpecialTags.EndReplaceTag;
        }

        public override string SearchValue()
        {
            return " " + _searchData + " ";
        }
    }
}