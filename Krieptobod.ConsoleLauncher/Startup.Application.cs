using KrieptoBod.Application;
using KrieptoBod.Application.Recommendators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBod.ConsoleLauncher
{
    public static class ApplicationServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationStartup(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITrader, Trader>();

            services.AddScoped<IRecommendator, RecommendatorRsi>();
            services.AddScoped<IRecommendator, RecommendatorSupport>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();

            return services;
        }
    }
}
