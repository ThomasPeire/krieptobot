using KrieptoBod.Application;
using KrieptoBod.Application.Recommendators;
using KrieptoBod.Infrastructure;
using KrieptoBod.Infrastructure.Exchange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBod.AzureFunction
{
    public static class ApplicationServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationStartup(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITrader, Trader>();
            services.AddScoped<IRepository, ExchangeRepository>();

            services.AddScoped<IRecommendator, RecommendatorRsi>();
            services.AddScoped<IRecommendator, RecommendatorSupport>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();

            return services;
        }
    }
}
