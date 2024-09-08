using Microsoft.Extensions.DependencyInjection;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Services;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddRadarSearchOptimizely(this IServiceCollection services)
        {
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IRadarContentService, RadarContentService>();
            services.AddScoped<IPropertyIndexService, PropertyIndexService>();

            return services;
        }
    }
}