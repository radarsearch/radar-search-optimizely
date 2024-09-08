namespace RadarSearchOptimizely.Search.Models;

/// <summary>
/// Models a radar search query.
/// </summary>
public class RadarQuery
{
    /// <summary>
    /// The default page number for searches.
    /// </summary>
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// The default page size for searches.
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// The default search language is None.
    /// </summary>
    public const string DefaultLanguage = "";

    /// <summary>
    /// The default type for searches.
    /// </summary>
    public const string DefaultSearchType = "ContentData";

    /// <summary>
    /// The default value for whether to allow an empty search string.
    /// </summary>
    public const bool DefaultAllowEmptySearchString = false;

    /// <summary>
    /// The default value for whether to filter content by visitor access level.
    /// </summary>
    public const bool DefaultFilterForVisitor = false;

    /// <summary>
    /// The word or phrase to search for. 
    /// </summary>
    public string SearchString { get; set; }

    /// <summary>
    /// The page number that is being requested. If Page is 2 and PageSize is 10, Radar should return results
    /// 10-19 inclusive.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The requested number of results per page. If Page is 2 and PageSize is 10, Radar should return results
    /// 10-19 inclusive.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Filter content by language
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// This property was previously named `AllowEmptyResult`. 
    /// If this is set to true, it seems like Radar will ignore the search string.
    /// If it is set to false, Radar will return an empty result for an empty search string
    /// </summary>
    public bool AllowEmptySearchString { get; set; }

    /// <summary>
    /// If this is set to true, search will be filtered to only show results the user has access to.
    /// </summary>
    public bool FilterForVisitor { get; set; }

    /// <summary>
    /// List of categories to search for. Results will be included if they have at least one of the given categories from each list.
    /// </summary>
    public List<List<string>> AdditiveFilters { get; set; }

    /// <summary>
    /// List of categories to search for. Results will be included if they have all of the given categories.
    /// </summary>
    public List<List<string>> SubtractiveFilters { get; set; }

    /// <summary>
    /// Data types to search for. The SearchType paramater is added to this list. 
    /// </summary>
    public List<string> SearchTypes { get; set; }

    /// <summary>
    /// Initializes a new RadarQuery with default values. If a searchString is not provided, allowEmptySearchString
    /// must be set to true. Otherwise Radar will return no results. 
    /// </summary>
    /// <param name="searchString">The word or phrase to search for. </param>
    /// <param name="page">The page number that is being requested.</param>
    /// <param name="pageSize">The requested number of results per page.</param>
    /// <param name="allowEmptySearchString">Whether to allow an empty search string.</param>
    /// <param name="filterForVisitor">Filters out content the user does not have access to.</param>
    /// <param name="categories">Compatibility parameter to avoid breaking change. Replaced by additiveFilters</param>
    /// <param name="additiveFilters">Additive categories to search for.</param>
    /// <param name="subtractiveFilters">Subtractive categories to search for.</param>
    /// <param name="searchTypes">Data types to search for.</param>
    public RadarQuery(
        string searchString = "",
        int page = DefaultPageNumber,
        int pageSize = DefaultPageSize,
        string language = DefaultLanguage,
        bool allowEmptySearchString = DefaultAllowEmptySearchString,
        bool filterForVisitor = DefaultFilterForVisitor,
        List<string> categories = null,
        List<string> searchTypes = null,
        List<List<string>> additiveFilters = null,
        List<List<string>> subtractiveFilters = null)
    {
        SearchString = searchString;
        Page = page;
        PageSize = pageSize;
        Language = language;
        AllowEmptySearchString = allowEmptySearchString;
        FilterForVisitor = filterForVisitor;
        AdditiveFilters = additiveFilters ?? (categories != null ? new List<List<string>> { categories } : new List<List<string>>());
        SubtractiveFilters = subtractiveFilters ?? new List<List<string>>();
        SearchTypes = searchTypes ?? new List<string> { DefaultSearchType };
    }
}