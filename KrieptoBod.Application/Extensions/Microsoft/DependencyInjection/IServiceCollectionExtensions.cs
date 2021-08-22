using KrieptoBod.Application;
using KrieptoBod.Application.Recommendators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KrieptoBod.Application.Extensions.Microsoft.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITrader, Trader>();

            services.AddScoped<IRecommendator, RecommendatorRsi>();
            services.AddScoped<IRecommendator, RecommendatorSupport>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();
        }
    }
}
