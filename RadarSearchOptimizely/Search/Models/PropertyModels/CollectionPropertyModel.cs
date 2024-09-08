using System.Collections;
using EPiServer.Core;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class CollectionPropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public CollectionPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;

            if (property == null || property.Value == null)
                return;

            try
            {
                var readOnlyCollectionObject = (ICollection)property.Value;
                if (readOnlyCollectionObject == null)
                    return;

                var objs = new ArrayList(readOnlyCollectionObject).ToArray();

                foreach (var obj in objs)
                {
                    _searchValue += obj.ToString().HtmlStrip() + " ";
                }
            }
            catch (Exception)
            {
            }

            _searchValue = SpecialTags.StartReplaceTag + " " + _searchValue.Trim().HtmlStrip() + " " + SpecialTags.EndReplaceTag;
        }

        public override string SearchValue()
        {
            return " " + _searchValue + " ";
        }
    }
}
