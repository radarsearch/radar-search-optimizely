using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class CategoryPropertyModel : PropertyModel
    {
        private readonly CategoryList _categories;

        public CategoryPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _categories = new CategoryList();
            if (property == null || property.Value == null)
                return;
            var cats = property.Value as CategoryList;
            if (cats != null)
                _categories = cats;
        }

        public override string SearchValue()
        {
            var searchValue = string.Empty;

            if (_categories == null)
                return string.Empty;

            var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();

            foreach (var catid in _categories)
            {
                var category = categoryRepository.Get(catid);
                if (category == null || string.IsNullOrEmpty(category.Name))
                    continue;
                searchValue += category.Name + " " + category.Description + " ";
            }

            return " " + searchValue.Trim().HtmlStrip() + " ";
        }
    }
}
