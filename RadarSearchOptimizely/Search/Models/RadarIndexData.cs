using System.Runtime.Serialization;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models.Enums;

namespace RadarSearchOptimizely.Search.Models
{
    [Serializable]
    [DataContract]
    public class RadarIndexData : EventArgs, IRadarIndexData
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public int ContentId { get; set; }
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string Unique { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public bool Deleted { get; set; }
        [DataMember]
        public bool HideInIndex { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string RelativeUrl { get; set; }
        [DataMember]
        public List<CategoryData> Categories { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public List<SearchMetaData> Metadata { get; set; }
        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public DateTime Updated { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public List<AccessControlEntry> Acl { get; set; }
        [DataMember]
        public string AclString { get; set; }
        [DataMember]
        public IndexStatus IndexStaus { get; set; }
        [DataMember]
        public string[] Keywords { get; set; }

        public RadarIndexData()
        {
            Id = null;
            Guid = Guid.Empty;
            Unique = string.Empty;
            Language = null;
            ContentId = -1;
            Metadata = new List<SearchMetaData>();
            Acl = new List<AccessControlEntry>();
            AclString = string.Empty;
            Categories = new List<CategoryData>();
        }

        public void SetId(Guid guid)
        {
            if (guid == Guid.Empty)
                return;
            Guid = guid;
            if (string.IsNullOrEmpty(Language))
                Id = CreateRadarSearchId(guid, Language);
        }

        public void SetId(string unique)
        {
            if (string.IsNullOrEmpty(unique))
                return;
            Unique = unique;
            if (string.IsNullOrEmpty(Language))
                Id = CreateRadarSearchId(unique, Language);
        }

        public void SetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return;
            Language = language.Replace("-", string.Empty);
            if (!string.IsNullOrEmpty(Unique))
                Id = CreateRadarSearchId(Unique, Language);
            if (Guid != Guid.Empty)
                Id = CreateRadarSearchId(Guid, Language);
        }

        public string GetAccessControlEntriesAsString()
        {
            return Acl.Where(x => x.Level >= AclLevel.Read)
                .Aggregate(string.Empty, (current, accessControlEntry) => current + (accessControlEntry.Name.FilterAclNames() + " "))
                .Trim(' ')
                .ToLower();
        }

        public string GetKeywordsAsIndexString()
        {
            if (Keywords == null)
                return string.Empty;

            var keywordstring = string.Empty;
            var counter = 1;
            foreach (var keyword in Keywords)
            {
                if (counter == Keywords.Length)
                    keywordstring += keyword;
                else
                    keywordstring += keyword + " # ";
                counter++;
            }
            return keywordstring.Trim(' ');
        }

        public static string[] CreateKeywordArrayFromString(string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return null;
            var split = keywords.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            var keywordlist = split.Select(s => s.Trim(' ')).ToList();
            return keywordlist.ToArray();
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