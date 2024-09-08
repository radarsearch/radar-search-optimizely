using System.Globalization;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class XhtmlStringPropertyModel : PropertyModel
    {
        private readonly string _searchValue;
        private readonly ICollection<IPropertyModel> _properties;

        public XhtmlStringPropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;
            _properties = new List<IPropertyModel>();
            if (property == null || property.Value == null || content == null)
                return;
            var val = property.Value as XhtmlString;
            if (val == null)
                return;

            if (firstLevel)
            {
                var fragments = val.Fragments.Where(x => x is EPiServer.Core.Html.StringParsing.ContentFragment).ToList();

                foreach (var stringFragment in fragments)
                {
                    var fragment = stringFragment as EPiServer.Core.Html.StringParsing.ContentFragment;
                    if (fragment == null)
                        continue;

                    IContent fragmentContent;
                    var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                    if (contentLoader == null || !contentLoader.TryGet(fragment.ContentLink, new CultureInfo(content.LanguageBranch()), out fragmentContent))
                        continue;
                    foreach (var prop in fragmentContent.Property)
                    {
                        _properties.Add(prop.CreatePropertyModel(fragmentContent, false, false));
                    }
                }
            }

            var textFragments = val.Fragments.Where(x => !(x is EPiServer.Core.Html.StringParsing.ContentFragment)).ToList();
            var textValues = textFragments.Aggregate(string.Empty, (current, stringFragment) => current + (stringFragment.GetViewFormat().HtmlStrip() + " "));
            _searchValue = textValues.Trim();
        }

        public override string SearchValue()
        {
            var retval = _properties.Where(model => model.Searchable).Aggregate(string.Empty, (current, model) => current + (" " + model.SearchValue() + " "));
            return " " + _searchValue.HtmlStrip() + " " + retval.HtmlStrip();
        }
    }
}