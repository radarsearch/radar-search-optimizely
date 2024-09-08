using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class RadarSearchModifyDataExtensions
    {
        public static void ConvertModifyData(this IRadarSearchModifyData radarSearchModifyData, IRadarIndexData radarIndexData)
        {
            if (radarSearchModifyData == null || radarIndexData == null)
            {
                return;
            }

            RadarSearchData radarSearchData = null;

            try
            {
                radarSearchData = radarSearchModifyData.SearchData();
            }
            catch (Exception)
            {
                // ignored
            }

            if (radarSearchData == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(radarSearchData.Unique))
            {
                radarIndexData.SetId(radarSearchData.Unique);
            }

            if (radarSearchData.Guid != Guid.Empty)
            {
                radarIndexData.SetId(radarSearchData.Guid);
            }

            if (!string.IsNullOrEmpty(radarSearchData.Language))
            {
                radarIndexData.SetLanguage(radarSearchData.Language);
            }

            if (!string.IsNullOrEmpty(radarSearchData.Title))
                radarIndexData.Title = radarSearchData.Title;

            if (!string.IsNullOrEmpty(radarSearchData.Description))
                radarIndexData.Description = radarSearchData.Description;

            if (!string.IsNullOrEmpty(radarSearchData.Text))
                radarIndexData.Text = radarSearchData.Text;

            if (!string.IsNullOrEmpty(radarSearchData.Type))
                radarIndexData.Type = radarSearchData.Type;

            radarIndexData.Deleted = radarSearchData.Delete;

            if (!string.IsNullOrEmpty(radarSearchData.Image))
                radarIndexData.Image = radarSearchData.Image;

            if (radarSearchData.Categories is { Count: > 0 })
                radarIndexData.Categories = radarSearchData.Categories;

            if (radarSearchData.Metadata is { Count: > 0 })
                radarIndexData.Metadata = radarSearchData.Metadata;

            if (!string.IsNullOrEmpty(radarSearchData.Url))
                radarIndexData.Url = radarSearchData.Url;

            if (!string.IsNullOrEmpty(radarSearchData.RelativeUrl))
                radarIndexData.RelativeUrl = radarSearchData.RelativeUrl;

            if (radarSearchData.Acl is { Count: > 0 })
                radarIndexData.Acl = radarSearchData.Acl;

            if (radarSearchData.Keywords is { Length: > 0 })
                radarIndexData.Keywords = radarSearchData.Keywords;

            if (radarSearchData.Created.Year > 1900)
                radarIndexData.Created = radarSearchData.Created;

            if (radarSearchData.Updated.Year > 1900)
                radarIndexData.Updated = radarSearchData.Updated;
        }
    }
}