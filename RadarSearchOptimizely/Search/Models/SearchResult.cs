namespace RadarSearchOptimizely.Search.Models
{
    public class SearchResult
    {
        public List<SearchResultItem> Items { get; set; }
        public List<string> CategoryFilter { get; set; }
        public List<string> SearchFields { get; set; }
        public int TotalHits { get; set; }
        public int Hits { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchType { get; set; }
        public bool AllowEmptyResult { get; set; }
        public string SearchString { get; set; }

        public SearchResult()
        {
            Items = new List<SearchResultItem>();
            CategoryFilter = new List<string>();
            SearchFields = new List<string>();
            TotalHits = 0;
            Hits = 0;
            TotalPages = 1;
            Page = 1;
            PageSize = 10;
            SearchType = "ContentData";
            AllowEmptyResult = false;
            SearchString = string.Empty;
        }
    }

    public class SearchResultItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Excerpt { get; set; }
        public DateTime Published { get; set; }
        public List<string> Categories { get; set; }
        public string ImageReference { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public Guid Guid { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime? Updated { get; set; }
        public string Type { get; set; }
        public string Acl { get; set; }
        public string Language { get; set; }
    }
}
