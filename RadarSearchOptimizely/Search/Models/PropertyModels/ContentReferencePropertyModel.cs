using System.Globalization;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class ContentReferencePropertyModel : PropertyModel
    {
        private readonly string _searchValue;

        public ContentReferencePropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _searchValue = string.Empty;
            try
            {
                if (property == null || property.Value == null)
                    return;
                var contentReference = property.Value as ContentReference;
                if (contentReference == null)
                    return;

                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var lang = content.LanguageBranch();
                IContent current;

                bool result;

                if (string.IsNullOrEmpty(lang) || lang == Const.GlobalLanguage)
                    result = contentRepository.TryGet(contentReference, out current);
                else
                    result = contentRepository.TryGet(contentReference, new CultureInfo(lang), out current);

                if (!result)
                    return;

                var text = current.Name;

                var radardata = current as IRadarSearchModifyData;

                if (radardata != null)
                {
                    text = string.IsNullOrEmpty(radardata.SearchData().Title) ? radardata.SearchData().Title : text;
                }
                if (!string.IsNullOrEmpty(text))
                {
                    _searchValue = SpecialTags.StartReplaceTag + " " + text.HtmlStrip() + " " + SpecialTags.EndReplaceTag;
                }
                else
                {
                    _searchValue = text;
                }
            }
            catch (Exception)
            {
            }
        }

        public override string SearchValue()
        {
            return " " + _searchValue + " ";
        }
    }
}
