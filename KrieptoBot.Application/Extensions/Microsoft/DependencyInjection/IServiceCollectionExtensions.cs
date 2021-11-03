﻿using KrieptoBot.Application.Recommendators;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace KrieptoBot.Application.Extensions.Microsoft.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITrader, Trader>();
            services.AddScoped<ISellManager, SellManager>();
            services.AddScoped<IBuyManager, BuyManager>();

            services.AddScoped<IRecommendator, RecommendatorRsi14PeriodInterval>();
            services.AddScoped<IRecommendator, RecommendatorRsi14Period4H>();

            services.AddScoped<IRecommendator, RecommendatorSupport>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();

            services.AddScoped<Indicators.IRsi, Indicators.Rsi>();

            services.AddSingleton<ITradingContext>(x =>
                new TradingContext()
                    .SetMarketsToWatch(new List<string> { "CHZ-EUR", "BTC-EUR", "ADA-EUR", "HOT-EUR", "1INCH-EUR", "ETH-EUR", "DOGE-EUR" }) // todo: appsettings
                    .SetInterval("5m")
                    .SetBuyMargin(20)
                    .SetSellMargin(-20));
        }
    }
}
