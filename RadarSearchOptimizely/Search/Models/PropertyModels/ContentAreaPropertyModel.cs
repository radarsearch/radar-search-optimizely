using System.Globalization;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class ContentAreaPropertyModel : PropertyModel
    {
        private readonly ICollection<IPropertyModel> _properties;

        public ContentAreaPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _properties = new List<IPropertyModel>();
            if (property == null || property.Value == null || content == null)
                return;
            var contentArea = property.Value as ContentArea;
            if (contentArea == null)
                return;
            var contents = contentArea.FindAllContentOfType<IContent>(new CultureInfo(content.LanguageBranch())).ToList();
            if (firstLevel)
            {
                foreach (var contentAreaContent in contents)
                {
                    if (!contentAreaContent.ShouldBeVisible())
                        continue;

                    foreach (var prop in contentAreaContent.Property)
                    {
                        _properties.Add(prop.CreatePropertyModel(contentAreaContent, false, false));
                    }
                }
            }
        }

        public override string SearchValue()
        {
            var retval = _properties.Where(model => model.Searchable).Aggregate(string.Empty, (current, model) => current + (" " + model.SearchValue() + " "));
            return " " + retval.HtmlStrip() + " ";
        }
    }
}