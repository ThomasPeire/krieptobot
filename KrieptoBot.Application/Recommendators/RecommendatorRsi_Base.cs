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

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorRsiBase : RecommendatorBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<RecommendatorRsiBase> _logger;
        private readonly int _periodToAverage;
        private readonly IRsi _rsiIndicator;
        private readonly ITradingContext _tradingContext;
        private readonly string _tradingContextInterval;

        protected RecommendatorRsiBase(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsiBase> logger, string tradingContextInterval,
            int periodToAverage, RecommendatorSettings recommendatorSettings) : base(recommendatorSettings, logger)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
            _tradingContext = tradingContext;
            _logger = logger;
            _tradingContextInterval = tradingContextInterval;
            _periodToAverage = periodToAverage;
        }

        protected override string Name => $"RSI{_periodToAverage} recommendator ({_tradingContextInterval})";

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var lastRsiValue = await GetLastRsiValue(market);

            _logger.LogDebug("Market {Market} - {Recommendator} RSI: {RsiValue}",
                market.Name.Value, Name, lastRsiValue.ToString("0.00"));

            var recommendatorScore = EvaluateRsiValue(lastRsiValue);

            return recommendatorScore;
        }

        private async Task<decimal> GetLastRsiValue(Market market)
        {
            var candles = await GetCandlesAsync(market);

            var rsiValues = GetRsiValues(candles);

            var (_, value) = rsiValues.OrderBy(x => x.Key).Last();
            return value;
        }

        private Dictionary<DateTime, decimal> GetRsiValues(IEnumerable<Candle> candles)
        {
            return _rsiIndicator.Calculate(candles, _periodToAverage).RsiValues;
        }

        private async Task<IEnumerable<Candle>> GetCandlesAsync(Market market)
        {
            return await _exchangeService.GetCandlesAsync(market.Name, _tradingContextInterval,
                _periodToAverage * 10,
                end: _tradingContext.CurrentTime);
        }

        private static RecommendatorScore EvaluateRsiValue(decimal rsiValue)
        {
            return rsiValue switch
            {
                <= 30 => new RecommendatorScore(RecommendationAction.Buy),
                >= 70 => new RecommendatorScore(RecommendationAction.Sell),
                _ => new RecommendatorScore(RecommendationAction.None)
            };
        }
    }
}
