using RadarSearchOptimizely.Search.Models;
using RadarSearchOptimizely.Search.Models.Enums;
using Index = RadarSearchOptimizely.Search.Models.Enums.Index;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class RadarSearchDataExtensions
    {
        public static RadarSearchData SetTitle(this RadarSearchData @radarSearchData, string title)
        {
            @radarSearchData.Title = title ?? string.Empty;
            return @radarSearchData;
        }

        public static RadarSearchData SetDescription(this RadarSearchData @radarSearchData, string description)
        {
            @radarSearchData.Description = description ?? string.Empty;
            return @radarSearchData;
        }

        public static RadarSearchData SetImage(this RadarSearchData @radarSearchData, string image)
        {
            @radarSearchData.Image = image ?? string.Empty;
            return @radarSearchData;
        }

        public static RadarSearchData SetText(this RadarSearchData @radarSearchData, string text)
        {
            @radarSearchData.Text = text ?? string.Empty;
            return @radarSearchData;
        }

        public static RadarSearchData SetCreated(this RadarSearchData @radarSearchData, DateTime created)
        {
            @radarSearchData.Created = created;
            return @radarSearchData;
        }

        public static RadarSearchData SetUpdated(this RadarSearchData @radarSearchData, DateTime updated)
        {
            @radarSearchData.Updated = updated;
            return @radarSearchData;
        }

        public static RadarSearchData RelativeUrl(this RadarSearchData @radarSearchData, string relativeUrl)
        {
            @radarSearchData.RelativeUrl = relativeUrl ?? string.Empty;
            return @radarSearchData;
        }

        public static RadarSearchData AddCategory(this RadarSearchData @radarSearchData, int id, string name, string description)
        {
            if (@radarSearchData.Categories == null)
                @radarSearchData.Categories = new List<CategoryData>();
            if (id < 0 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                return @radarSearchData;
            @radarSearchData.Categories.Add(new CategoryData { Id = id, Name = name, Description = description });
            return @radarSearchData;
        }

        public static RadarSearchData AddAcl(this RadarSearchData @radarSearchData, AccessControlEntry accessControlEntry)
        {
            if (@radarSearchData.Acl == null)
                @radarSearchData.Acl = new List<AccessControlEntry>();
            if (accessControlEntry == null)
                return @radarSearchData;
            @radarSearchData.Acl.Add(accessControlEntry);
            return @radarSearchData;
        }

        public static RadarSearchData AddMetadata(this RadarSearchData @radarSearchData, string fieldname, string value, float boost = 1.0f, int extremeBoost = 1, Index fieldIndex = Index.ANALYZED, Store fieldStore = Store.YES)
        {
            if (@radarSearchData.Metadata == null)
                @radarSearchData.Metadata = new List<SearchMetaData>();
            if (string.IsNullOrEmpty(fieldname) || string.IsNullOrEmpty(value))
                return @radarSearchData;

            var md = new SearchMetaData
            {
                FieldName = fieldname,
                Value = value,
                Boost = boost,
                ExtremeBoost = extremeBoost,
                FieldIndex = fieldIndex,
                FieldStore = fieldStore
            };
            @radarSearchData.Metadata.Add(md);
            return @radarSearchData;
        }

        public static RadarSearchData SetDelete(this RadarSearchData @radarSearchData, bool delete)
        {
            @radarSearchData.Delete = delete;
            return @radarSearchData;
        }

        public static RadarSearchData SetFileContent(this RadarSearchData @radarSearchData, bool fileContent)
        {
            @radarSearchData.FileContent = fileContent;
            return @radarSearchData;
        }

        public static RadarSearchData AddKeywords(this RadarSearchData @radarSearchData, string[] keywords)
        {
            if (keywords == null)
                return @radarSearchData;
            @radarSearchData.Keywords = keywords;
            return @radarSearchData;
        }

        public static RadarSearchData FindContentType(this RadarSearchData @radarSearchData, object obj)
        {
            if (obj == null)
                return @radarSearchData;
            var currentType = obj.GetType();
            var concreteTypes = currentType.GetParentTypes().Where(x => !x.IsInterface).ToList();
            if (concreteTypes.Count < 1)
                return @radarSearchData;
            var types = concreteTypes.Select(concreteType => concreteType.Name).ToList();
            if (!types.Any(x => x.Equals(currentType.Name)))
                types.Add(currentType.Name);
            @radarSearchData.Type = types.Aggregate(string.Empty, (current, type) => current + ("[" + type + "] ")).Trim(' ');
            return @radarSearchData;
        }

        public static RadarSearchData SetLanguage(this RadarSearchData @radarSearchData, string language)
        {
            if (string.IsNullOrEmpty(language))
                return @radarSearchData;
            @radarSearchData.Language = language.Replace("-", string.Empty);
            if (!string.IsNullOrEmpty(@radarSearchData.Unique))
                @radarSearchData.Id = CreateRadarSearchId(@radarSearchData.Unique, @radarSearchData.Language);
            if (@radarSearchData.Guid != Guid.Empty)
                @radarSearchData.Id = CreateRadarSearchId(@radarSearchData.Guid, @radarSearchData.Language);
            return @radarSearchData;
        }

        public static RadarSearchData SetId(this RadarSearchData @radarSearchData, string unique)
        {
            if (string.IsNullOrEmpty(unique))
                return @radarSearchData;
            @radarSearchData.Unique = unique;
            if (string.IsNullOrEmpty(@radarSearchData.Language))
                @radarSearchData.Id = CreateRadarSearchId(unique, @radarSearchData.Language);
            return @radarSearchData;
        }

        public static RadarSearchData SetId(this RadarSearchData @radarSearchData, Guid guid)
        {
            if (guid == Guid.Empty)
                return @radarSearchData;
            @radarSearchData.Guid = guid;
            if (string.IsNullOrEmpty(@radarSearchData.Language))
                @radarSearchData.Id = CreateRadarSearchId(guid, @radarSearchData.Language);
            return @radarSearchData;
        }

        public static string CreateRadarSearchId(Guid guid, string language)
        {
            if (guid != Guid.Empty && !string.IsNullOrEmpty(language))
                return guid + "|" + language.Replace("-", string.Empty);
            return null;
        }

        public static string CreateRadarSearchId(string uniqueId, string language)
        {
            if (!string.IsNullOrEmpty(uniqueId) && !string.IsNullOrEmpty(language))
                return uniqueId + "|" + language.Replace("-", string.Empty);
            return null;
        }
    }
}