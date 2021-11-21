﻿using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            services.AddScoped<IRecommendator, RecommendatorProfitPercentage>();

            services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();

            services.AddScoped<IRsi, Rsi>();

            services.AddSingleton<ITradingContext>(x =>
            {
                var tradingSettings = x.GetRequiredService<IOptions<TradingSettings>>().Value;

                return new TradingContext(new DateTimeProvider())
                    .SetMarketsToWatch(tradingSettings.MarketsToWatch)
                    .SetInterval(tradingSettings.Interval)
                    .SetBuyMargin(tradingSettings.BuyMargin)
                    .SetSellMargin(tradingSettings.SellMargin)
                    .SetIsSimulation(tradingSettings.IsSimulation);
            });
        }
    }
}
