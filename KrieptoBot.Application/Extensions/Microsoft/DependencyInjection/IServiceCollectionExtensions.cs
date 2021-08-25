using KrieptoBot.Application.Recommendators;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBot.Application.Extensions.Microsoft.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITrader, Trader>();

            services.AddScoped<IRecommendator, RecommendatorRsi14>();
            services.AddScoped<IRecommendator, RecommendatorSupport>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();

            services.AddScoped<Indicators.IRsi, Indicators.Rsi>();
        }
    }
}
