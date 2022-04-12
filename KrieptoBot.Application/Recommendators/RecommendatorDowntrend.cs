using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Constants;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorDownTrend : RecommendatorBase
{
    protected override string Name => "Downtrend recommendator";
    private readonly ILogger<RecommendatorDownTrend> _logger;
    private readonly IExchangeService _exchangeService;
    private readonly ITradingContext _tradingContext;
    private readonly IExponentialMovingAverage _ema;

    public RecommendatorDownTrend(IOptions<RecommendatorSettings> recommendatorSettings,
        ILogger<RecommendatorDownTrend> logger,
        IExchangeService exchangeService, ITradingContext tradingContext, IExponentialMovingAverage ema)
        : base(recommendatorSettings.Value, logger)
    {
        _logger = logger;
        _exchangeService = exchangeService;
        _tradingContext = tradingContext;
        _ema = ema;
    }

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var trendCandles = await GetTrendCandles(market);
        var intervalCandles = (await GetIntervalCandles(market)).ToList();

        var ema55 = _ema.Calculate(trendCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value), 55);

        var intervalCandlesDictionary = intervalCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value);
        var ema55ValuesToCheckInterpolated = CalculateEma55ValuesInterpolated(ema55,
            intervalCandlesDictionary.Select(x => x.Key).ToList());

        var allPricesBelowEma = intervalCandlesDictionary.OrderByDescending(x => x.Key)
            .Take(RecommendatorSettings.DownTrendRecommendatorNumberOfConsecutiveCandles)
            .All(x => PriceBelowEmaValue(x, ema55ValuesToCheckInterpolated));

        _logger.LogDebug(
            "Market {Market} - {Recommendator} Downtrend detected: {DownTrendDetected}",
            market.Name.Value, Name, allPricesBelowEma);

        var balance = await GetAvailableBalanceForAsset(market.Name.BaseSymbol);
        var thereIsOpenBalance = market.MinimumBaseAmount <= balance;

        _logger.LogDebug(
            "Market {Market} - {Recommendator} Sufficient open balance: {OpenBalance}, score {IncludeOrNot} included in final score",
            market.Name.Value, Name, thereIsOpenBalance, thereIsOpenBalance ? "won't be" : "will be");

        return allPricesBelowEma
            ? new RecommendatorScore(RecommendationAction.Sell, !thereIsOpenBalance)
            : new RecommendatorScore(RecommendationAction.None, false);
    }

    private IDictionary<DateTime, decimal> CalculateEma55ValuesInterpolated(
        IEnumerable<KeyValuePair<DateTime, decimal>> ema55ValuesToCheck,
        List<DateTime> dateTimesToInterpolate)
    {
        var emaValues = ema55ValuesToCheck.ToList();

        var interpolatedEmaValues = new Dictionary<DateTime, decimal>();

        foreach (var dateTime in dateTimesToInterpolate)
        {
            var firstValue = emaValues.Where(x => x.Key <= dateTime).OrderByDescending(x => x.Key).FirstOrDefault();
            var lastValue = emaValues.Where(x => x.Key >= dateTime).OrderBy(x => x.Key).FirstOrDefault();
            lastValue = lastValue.Equals(default(KeyValuePair<DateTime, decimal>))
                ? firstValue
                : lastValue;

            var difference = lastValue.Value - firstValue.Value;

            var numberOfMinutesBetweenFirstAndLast = (lastValue.Key - firstValue.Key).TotalMinutes;
            var numberOfMinutesSinceFirst = (dateTime - firstValue.Key).TotalMinutes;

            if (numberOfMinutesBetweenFirstAndLast == 0 || numberOfMinutesSinceFirst == 0)
            {
                interpolatedEmaValues.Add(dateTime, firstValue.Value);
                continue;
            }

            var percentageOfMinutesSinceFirst = numberOfMinutesSinceFirst / numberOfMinutesBetweenFirstAndLast;

            interpolatedEmaValues.Add(dateTime,
                firstValue.Value + (decimal)percentageOfMinutesSinceFirst * difference);
        }

        return interpolatedEmaValues;
    }

    private static bool PriceBelowEmaValue(KeyValuePair<DateTime, decimal> price,
        IDictionary<DateTime, decimal> emaValues)
    {
        return price.Value < emaValues[price.Key];
    }

    private async Task<IEnumerable<Candle>> GetIntervalCandles(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name,
            _tradingContext.Interval,
            RecommendatorSettings.DownTrendRecommendatorNumberOfConsecutiveCandles,
            end: _tradingContext.CurrentTime);
    }

    private async Task<IEnumerable<Candle>> GetTrendCandles(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name,
            RecommendatorSettings.DownTrendRecommendatorInterval, 200,
            end: _tradingContext.CurrentTime);
    }

    private async Task<Amount> GetAvailableBalanceForAsset(Symbol asset)
    {
        var availableBalance = await _exchangeService.GetBalanceAsync(asset);

        return availableBalance != null
            ? new Amount(Math.Max(availableBalance.Available.Value - availableBalance.InOrder.Value, 0))
            : Amount.Zero;
    }

    private static int GetIntervalInMinutes(string interval)
    {
        return interval switch
        {
            "1m" => 1,
            "5m" => 5,
            "15m" => 15,
            "30m" => 30,
            "1h" => 60,
            "2h" => 120,
            "4h" => 240,
            "6h" => 360,
            "8h" => 480,
            "12h" => 720,
            "1d" => 1440,
            _ => 240
        };
    }
}
