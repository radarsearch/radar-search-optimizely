using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Services;

namespace RadarSearchOptimizely.Search.Configuration;

public static class RadarSearchConfiguration
{
    public static void AddRadarSearch(this IServiceCollection services)
    {
        services.AddTransient<ISearchService, SearchService>();
        services.AddTransient<IExtendedContentRepository, ExtendedContentService>();
        services.AddTransient<IRadarContentService, RadarContentService>();
        services.AddTransient<IPropertyIndexService, PropertyIndexService>();
    }

    public static void AddRadarAdminUi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/index", [Authorize(Roles = "Administrators")] ([FromServices] ISearchService searchService) => Task.FromResult(searchService.GetAllIndexData()));
    }
}