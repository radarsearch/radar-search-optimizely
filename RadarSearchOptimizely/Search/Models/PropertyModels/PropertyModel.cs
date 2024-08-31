using EPiServer.Core;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public abstract class PropertyModel : IPropertyModel
    {
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool IsBlockChild { get; set; }

        private PropertyModel() { }

        protected PropertyModel(PropertyData propertyData, IContent content, bool firstLevel, bool isBlockChild)
        {
            if (propertyData == null)
            {
                Name = string.Empty;
                Searchable = false;
                return;
            }

            Name = propertyData.Name;
            IsBlockChild = isBlockChild;

            if (IsBlockChild)
            {
                Searchable = true;
                return;
            }

            if (content == null)
            {
                // It is a block property
                Searchable = true;
                return;
            }

            Searchable = propertyData.IsSearchable(content);
        }

        public abstract string SearchValue();
    }
}
