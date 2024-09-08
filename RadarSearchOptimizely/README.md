# :toolbox: How to

## Install nuget package

Install nuget package from [here]('https://www.nuget.org/packages/RadarSearchOptimizely')

## Installation

Add the services to Startup.cs for dependency injection

``` cs
using RadarSearchOptimizely.Search.Extensions;

public class Startup
{
    ...    
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddRadarSearchOptimizely();
    }
}
```

Add the following section into appsettings.json

``` json
"RadarSearchConfiguration": {
    "IgnoreContentTypes": "SysRecycleBin, SysContentFolder, SysRecycleBin, ImageFile, VideoFile, GenericMedia, BlockData",
    "RadarApiUrl": "http://<Standalone-RadarSearch-URL>/api/radar/"
}
```

> Add the URL for the IIS hosted RadarSearch standalone application.
> RadarSearch Standalone Application: https://github.com/bouvet-bergen/RadarSearch/tree/StandaloneApp

## Content

The package includes:

- A scheduled job that will parse all content and send it the RadarSearch Standalone application for indexing
- An event handler that sends indexing data to RadarSearch whenever content is being published, moved or deleted
- An interface, ISearchService, that is used to send API calls from the Optimizely application to the RadarSearch Standalone application for searching and indexing

## Example usage

``` cs
using RadarSearchOptimizely.Search;

public class SearchPageController : PageControllerBase<SearchPage>
{
    private readonly ISearchService _searchService;

    public SearchPageController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<ViewResult> Index(SearchPage currentPage, string q, int page = 1)
    {
        var query = new RadarQuery()
        {
            SearchString = q,
            Page = page,
            PageSize = 10,
            SearchType = "SomePageType",
            AllowEmptySearchString = false
        };

        var model = new SearchContentModel(currentPage)
        {
            RadarSearchResult = await _searchService.Search(query),
            SearchQuery = q
        };

        return View(model);
    }
}
```

## Custom search data (optional)

``` cs
public class SitePageData : PageData, IRadarSearchModifyData
{
    [Display(GroupName = "MetaData", Order = 20)]
    [Searchable(false)]
    [UIHint(UIHint.Image)]
    [ScaffoldColumn(false)]
    public virtual ContentReference PageImage { get; set; }

    [Display(GroupName = SystemTabNames.Content, Order = 3)]
    [CultureSpecific]
    [Searchable(true)]
    public virtual string PageTitle { get; set; }

    [Display(GroupName = SystemTabNames.Content, Order = 10)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string PageDescription { get; set; }

    #region RadarSearch

    public virtual RadarSearchData SearchData()
    {
        return RadarSearchData.Create()
            .SetTitle(PageTitle ?? string.Empty)
            .SetDescription(PageDescription ?? string.Empty)
            .SetImage(PageImage != null ? PageImage.ID.ToString(CultureInfo.InvariantCulture) : string.Empty);
    }

    #endregion
}
```

> Note: Remember to inherit the IRadarSearchModifyData interface
