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
        var intervalCandles = await GetIntervalCandles(market);

        var ema55 = _ema.Calculate(trendCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value), 55).ToList();
        var ema55ValuesToCheck = ema55.OrderByDescending(x => x.Key)
            .Take(RecommendatorSettings.DownTrendRecommendatorNumberOfConsecutiveCandles).ToList();

        var intervalCandlesDictionary = intervalCandles.ToDictionary(c => c.TimeStamp, c => c.Close.Value);
        var allPricesBelowEma = ema55ValuesToCheck.All(x => PriceBelowEmaValue(x, intervalCandlesDictionary));

        _logger.LogDebug(
            "Market {Market} - {Recommendator} Downtrend detected: {DownTrendDetected}",
            market.Name.Value, Name, allPricesBelowEma);

        return allPricesBelowEma
                ? new RecommendatorScore(RecommendationAction.None, false)
                : new RecommendatorScore(RecommendationAction.Sell);
    }

    private static bool PriceBelowEmaValue(KeyValuePair<DateTime, decimal> emaValue,
        IReadOnlyDictionary<DateTime, decimal> intervalCandlesDictionary)
    {
        return emaValue.Value > intervalCandlesDictionary[emaValue.Key];
    }

    private async Task<IEnumerable<Candle>> GetIntervalCandles(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name,
            _tradingContext.Interval, 200,
            end: _tradingContext.CurrentTime);
    }

    private async Task<IEnumerable<Candle>> GetTrendCandles(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name,
            RecommendatorSettings.DownTrendRecommendatorInterval,
            RecommendatorSettings.DownTrendRecommendatorNumberOfConsecutiveCandles,
            end: _tradingContext.CurrentTime);
    }
}
