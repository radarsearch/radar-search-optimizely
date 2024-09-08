using System.Text;
using EPiServer.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Extensions;
using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _radarApiUrl;
        private readonly string _radarApiUser;
        private readonly string _radarApiKey;

        public SearchService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _radarApiUrl = configuration.GetSection("RadarSearchConfiguration").GetSection("RadarApiUrl").Value;
            _radarApiUser = configuration.GetSection("RadarSearchConfiguration").GetSection("RadarApiUser").Value;
            _radarApiKey = configuration.GetSection("RadarSearchConfiguration").GetSection("RadarApiKey").Value;
        }

        [Obsolete("This method is deprecated. Use Search(RadarQuery) instead.", false)]
        public Task<SearchResult> Search(SearchResult query)
            => Search(new RadarQuery
            {
                SearchString = query.SearchString,
                Page = query.Page,
                PageSize = query.PageSize,
                SearchTypes = new List<string> { query.SearchType },
                AllowEmptySearchString = query.AllowEmptyResult
            });

        // This method can be significantly simplified and improved if we manage to move this filtering logic to the 
        // RadarSearch standalone application where it belongs. For now it makes sense to do it here because it allows
        // us to move the filtering later without affecting applications using the filters. 
        public async Task<SearchResult> Search(RadarQuery query)
        {
            if (query.Page < 1)
                throw new ArgumentException("Page can not be less than 1");

            var shouldFilter = query.AdditiveFilters != null || query.SubtractiveFilters != null || query.FilterForVisitor;
            var pageSize = shouldFilter ? 7500 : query.PageSize;
            var page = shouldFilter ? 1 : query.Page;
            var searchType = query.SearchTypes.Count == 1
                ? query.SearchTypes.First() : RadarQuery.DefaultSearchType;

            var request = CreateAuthenticatedHttpRequest(
                HttpMethod.Get,
                $"{_radarApiUrl}search/" +
                $"?query={query.SearchString}" +
                $"&page={page}" +
                $"&pageSize={pageSize}" +
                $"&searchType={searchType}" +
                $"&allowEmptyResult={query.AllowEmptySearchString}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return EmptyResult(query);

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await System.Text.Json.JsonSerializer.DeserializeAsync<SearchResult>(responseStream);

            result = result is null ? EmptyResult(query) : FilterAndPaginate(result, query);

            return result;
        }

        private SearchResult FilterAndPaginate(SearchResult result, RadarQuery query)
        {
            var items = result.Items.AsEnumerable();
            if (query.FilterForVisitor)
                items = items.Where(SearchResultExtensions.VisitorCanSeeItem);
            items = query.AdditiveFilters
                .Where(additiveFilter => additiveFilter != null && additiveFilter.Any())
                .Aggregate(items,
                    (current,
                        subtractiveFilter) => current.Where(ContainsAtLeastOne(subtractiveFilter)));
            items = query.SubtractiveFilters
                .Where(subtractiveFilter => subtractiveFilter != null && subtractiveFilter.Any())
                .Aggregate(items,
                    (current,
                        subtractiveFilter) => current.Where(ContainsAll(subtractiveFilter)));
            if (query.SearchTypes.Count > 1)
                items = items.Where(PageTypesIntersect(query.SearchTypes));
            if (!string.IsNullOrEmpty(query.Language))
                items = items.Where(HasLanguageOrGlobal(query.Language));

            var resultList = items.ToList();
            result.Items = resultList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            result.PageSize = query.PageSize;
            result.Page = query.Page;
            result.Hits = resultList.Count;
            result.TotalPages = (result.Hits / query.PageSize) + 1;

            return result;
        }

        private static Func<SearchResultItem, bool> HasLanguageOrGlobal(string queryLanguage) => item =>
            string.Equals(item.Language.Replace("-", ""), queryLanguage.Replace("-", ""),
                StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(item.Language.ToLower(), "global");

        private static Func<SearchResultItem, bool> ContainsAll(IReadOnlyCollection<string> categories)
            => item => categories.Intersect(item.Categories).Count() == categories.Count;

        private static Func<SearchResultItem, bool> ContainsAtLeastOne(IReadOnlyCollection<string> categories)
            => item => item.Categories.Intersect(categories).Any();

        private static Func<SearchResultItem, bool> PageTypesIntersect(IReadOnlyCollection<string> types)
            => item =>
            {
                var typesArr = item.Type.Split(" ");

                return typesArr.Intersect(types).Any();
            };

        private static SearchResult EmptyResult(RadarQuery query)
            => new SearchResult
            {
                SearchString = query.SearchString,
                Page = query.Page,
                PageSize = query.PageSize,
                AllowEmptyResult = query.AllowEmptySearchString
            };

        public async Task<string> SendIndexData(List<IRadarIndexData> indexData)
        {
            var client = _clientFactory.CreateClient();
            client.Timeout = new TimeSpan(2, 0, 0);

            var logger = LogManager.GetLogger(typeof(SearchService));

            const int batchSize = 1000;
            var batches = indexData.Chunk(batchSize).ToList();

            try
            {
                foreach (var it in batches.Select((x, i) => new { Value = x, Index = i }))
                {
                    var request = CreateAuthenticatedHttpRequest(HttpMethod.Post, $"{_radarApiUrl}AddToQueue/");

                    logger.Information($"Queueing batch {it.Index + 1} out of {batches.Count}");
                    var batch = it.Value.ToList();
                    var json = JsonConvert.SerializeObject(batch);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    request.Content = data;
                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.Error("Error queueing batch", response.Content);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error($"Indexing failed with error message: {e.Message}", e);
            }

            var queueStatus = await RunQueue();

            return $"Done indexing {indexData.Count} items. Queue status: {queueStatus}";
        }

        public async Task<List<RadarIndexData>> GetAllIndexData()
        {
            var allIndexData = new List<RadarIndexData>();

            var request = CreateAuthenticatedHttpRequest(HttpMethod.Get, $"{_radarApiUrl}GetAllIndexData/");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                allIndexData = await System.Text.Json.JsonSerializer.DeserializeAsync<List<RadarIndexData>>(responseStream);
            }

            return allIndexData;
        }

        public async Task<string> RunQueue()
        {
            var client = _clientFactory.CreateClient();
            client.Timeout = new TimeSpan(0, 30, 0);
            var runQueueRequest = CreateAuthenticatedHttpRequest(HttpMethod.Get, $"{_radarApiUrl}StartRunningQueue/");
            var response = await client.SendAsync(runQueueRequest);
            return response.IsSuccessStatusCode
                ? "Queue ran successfully"
                : "Error running queue";
        }

        private HttpRequestMessage CreateAuthenticatedHttpRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);

            var authenticationString = $"{_radarApiUser}:{_radarApiKey}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);

            return request;
        }
    }
}