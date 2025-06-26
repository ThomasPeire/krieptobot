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

public class RecommendatorRsiEma(
    IExchangeService exchangeService,
    IRsi rsiIndicator,
    ITradingContext tradingContext,
    ILogger<RecommendatorRsiEma> logger,
    IOptions<RecommendatorSettings> recommendatorSettings)
    : RecommendatorBase(recommendatorSettings.Value, logger)
{
    private readonly int _rsiEmaRecommendatorEmaPeriod = recommendatorSettings.Value.RsiEmaRecommendatorEmaPeriod;
    private readonly int _rsiEmaRecommendatorRsiPeriod = recommendatorSettings.Value.RsiEmaRecommendatorRsiPeriod;

    private readonly decimal _rsiEmaRecommendatorSellSignalThreshold =
        recommendatorSettings.Value.RsiEmaRecommendatorSellSignalThreshold;

    private readonly decimal _rsiEmaRecommendatorBuySignalThreshold =
        recommendatorSettings.Value.RsiEmaRecommendatorBuySignalThreshold;

    protected override string Name =>
        $"RSI{_rsiEmaRecommendatorRsiPeriod} EMA {_rsiEmaRecommendatorEmaPeriod} recommendator";

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var emaValue = await GetLastEmaValue(market);

        logger.LogDebug("Market {Market} - {Recommendator} EMA: {EmaValue}",
            market.Name.Value, Name, emaValue.ToString("0.00"));

        var recommendatorScore = EvaluateEmaValue(emaValue);

        return recommendatorScore;
    }

    private async Task<decimal> GetLastEmaValue(Market market)
    {
        var candles = await GetCandlesAsync(market);

        var emaValues = GetEmaValues(candles);

        var (_, value) = emaValues.OrderBy(x => x.Key).Last();
        return value;
    }

    private Dictionary<DateTime, decimal> GetEmaValues(IEnumerable<Candle> candles)
    {
        return rsiIndicator.CalculateWithEma(candles, _rsiEmaRecommendatorRsiPeriod, _rsiEmaRecommendatorEmaPeriod)
            .EmaValues;
    }

    private async Task<IEnumerable<Candle>> GetCandlesAsync(Market market)
    {
        return await exchangeService.GetCandlesAsync(market.Name, tradingContext.Interval,
            end: tradingContext.CurrentTime);
    }

    private RecommendatorScore EvaluateEmaValue(decimal emaValue)
    {
        if (emaValue <= _rsiEmaRecommendatorBuySignalThreshold)
        {
            return new RecommendatorScore(RecommendationAction.Buy);
        }

        if (emaValue >= _rsiEmaRecommendatorSellSignalThreshold)
        {
            return new RecommendatorScore(RecommendationAction.Sell);
        }

        return new RecommendatorScore(RecommendationAction.None);
    }
}