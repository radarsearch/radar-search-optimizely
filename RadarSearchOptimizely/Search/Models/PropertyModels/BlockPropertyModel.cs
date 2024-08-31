using EPiServer.Core;
using RadarSearchOptimizely.Search.Attributes;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class BlockPropertyModel : PropertyModel
    {
        private readonly ICollection<IPropertyModel> _properties;

        public BlockPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _properties = new List<IPropertyModel>();
            if (property == null || property.Value == null)
                return;
            var block = property.Value as BlockData;
            if (block == null)
                return;

            if (firstLevel)
            {
                var attribute = block.GetType().GetAttribute<RadarLocalBlockAttribute>();
                if (attribute != null && attribute.Searchable == false)
                {
                    Searchable = false;
                    return;
                }
                Searchable = true;
                foreach (var p in block.Property)
                {
                    _properties.Add(p.CreatePropertyModel(content, false, true));
                }
            }
        }

        public override string SearchValue()
        {
            var retval = _properties.Aggregate(string.Empty, (current, model) => current + (model.SearchValue() + " "));
            return " " + retval.HtmlStrip() + " ";
        }
    }

    public static class ExtensionAttributeType
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return att != null ? valueSelector(att) : default(TValue);
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
        }


    }
}
