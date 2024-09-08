using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class SearchResultExtensions
    {
        [Obsolete("This is handled internally. Set `FilterForVisitor` to true in the RadarQuery.", false)]
        public static SearchResult FilterForVisitor(this SearchResult searchResult)
        {
            searchResult.Items = searchResult.Items
                .Where(VisitorCanSeeItem)
                .ToList();

            return searchResult;
        }

        internal static bool VisitorCanSeeItem(SearchResultItem item)
        {
            if (string.IsNullOrEmpty(item?.Acl))
            {
                return false;
            }

            var aclSplit = item.Acl.Split(" ");

            var isInRole = aclSplit.Any(role => EPiServer.Security.PrincipalInfo.CurrentPrincipal.IsInRole(role));

            return isInRole;
        }
    }
}