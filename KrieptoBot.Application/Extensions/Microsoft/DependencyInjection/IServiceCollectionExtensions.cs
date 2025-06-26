using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITrader, Trader>();
        services.AddScoped<ISellManager, SellManager>();
        services.AddScoped<IBuyManager, BuyManager>();

        services.AddScoped<IRecommendator, RecommendatorRsi14PeriodInterval>();
        services.AddScoped<IRecommendator, RecommendatorRsi14Period4H>();
        services.AddScoped<IRecommendator, RecommendatorProfitPercentage>();
        services.AddScoped<IRecommendator, RecommendatorMacd>();
        services.AddScoped<IRecommendator, RecommendatorDownTrend>();
        services.AddScoped<IRecommendator, RecommendatorRsiEma>();

        services.AddScoped<IRecommendatorSorter, RecommendatorSorter>();
        services.AddScoped<IRecommendationCalculator, RecommendationCalculator>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IRsi, Rsi>();
        services.AddScoped<IMacd, Macd>();
        services.AddScoped<IExponentialMovingAverage, ExponentialMovingAverage>();

        services.AddSingleton<ITradingContext>(x =>
        {
            var tradingSettings = x.GetRequiredService<IOptions<TradingSettings>>().Value;
            var dateTimeProvider = x.GetRequiredService<IDateTimeProvider>();
            var tradingContext = new TradingContext(dateTimeProvider)
                .SetMarketsToWatch(tradingSettings.MarketsToWatch)
                .SetInterval(tradingSettings.Interval)
                .SetBuyMargin(tradingSettings.BuyMargin)
                .SetSellMargin(tradingSettings.SellMargin)
                .SetIsSimulation(tradingSettings.IsSimulation)
                .SetPollingInterval(tradingSettings.PollingIntervalInMinutes)
                .SetBuyCoolDownPeriod(tradingSettings.BuyCoolDownPeriodInMinutes);
            _ = tradingContext.SetCurrentTime();
            return tradingContext;
        });
    }
}