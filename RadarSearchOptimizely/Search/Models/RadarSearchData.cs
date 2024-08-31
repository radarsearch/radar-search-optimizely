using RadarSearchOptimizely.Search.Contracts;

namespace RadarSearchOptimizely.Search.Models
{
    public class RadarSearchData : IRadarSearchData
    {
        public string Id { get; internal set; }
        public Guid Guid { get; internal set; }
        public string Unique { get; internal set; }
        public string Language { get; internal set; }

        public string Text { get; internal set; }
        public DateTime Updated { get; internal set; }
        public DateTime Created { get; internal set; }
        public string Url { get; internal set; }
        public string RelativeUrl { get; internal set; }
        public string Type { get; internal set; }
        public List<AccessControlEntry> Acl { get; internal set; }

        public string Title { get; internal set; }
        public string Description { get; internal set; }
        public string Image { get; internal set; }
        public List<CategoryData> Categories { get; set; }
        public List<SearchMetaData> Metadata { get; internal set; }
        public bool Delete { get; internal set; }
        public bool FileContent { get; internal set; }
        public string[] Keywords { get; internal set; }

        public static RadarSearchData Create()
        {
            return CreateDefault();
        }

        private static RadarSearchData CreateDefault()
        {
            var radarSearchData = new RadarSearchData
            {
                Text = string.Empty,
                Updated = DateTime.Now,
                Created = new DateTime(),
                Url = string.Empty,
                RelativeUrl = string.Empty,
                Type = string.Empty,
                Acl = new List<AccessControlEntry>(),
                Title = string.Empty,
                Description = string.Empty,
                Image = string.Empty,
                Categories = new List<CategoryData>(),
                Metadata = new List<SearchMetaData>(),
                Delete = false,
                FileContent = false,
                Keywords = null
            };
            return radarSearchData;
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
    }
}
