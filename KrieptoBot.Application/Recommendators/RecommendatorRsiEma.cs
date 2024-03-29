﻿using System;
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

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsiEma : RecommendatorBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<RecommendatorRsiEma> _logger;
        private readonly IRsi _rsiIndicator;
        private readonly ITradingContext _tradingContext;
        private readonly int _rsiEmaRecommendatorEmaPeriod;
        private readonly int _rsiEmaRecommendatorRsiPeriod;
        private readonly decimal _rsiEmaRecommendatorSellSignalThreshold;
        private readonly decimal _rsiEmaRecommendatorBuySignalThreshold;

        public RecommendatorRsiEma(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsiEma> logger,
            IOptions<RecommendatorSettings> recommendatorSettings) : base(recommendatorSettings.Value, logger)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
            _tradingContext = tradingContext;
            _logger = logger;
            _rsiEmaRecommendatorEmaPeriod = recommendatorSettings.Value.RsiEmaRecommendatorEmaPeriod;
            _rsiEmaRecommendatorRsiPeriod = recommendatorSettings.Value.RsiEmaRecommendatorRsiPeriod;
            _rsiEmaRecommendatorSellSignalThreshold =
                recommendatorSettings.Value.RsiEmaRecommendatorSellSignalThreshold;
            _rsiEmaRecommendatorBuySignalThreshold = recommendatorSettings.Value.RsiEmaRecommendatorBuySignalThreshold;
        }

        protected override string Name =>
            $"RSI{_rsiEmaRecommendatorRsiPeriod} EMA {_rsiEmaRecommendatorEmaPeriod} recommendator";

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var emaValue = await GetLastEmaValue(market);

            _logger.LogDebug("Market {Market} - {Recommendator} EMA: {EmaValue}",
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
            return _rsiIndicator.CalculateWithEma(candles, _rsiEmaRecommendatorRsiPeriod, _rsiEmaRecommendatorEmaPeriod)
                .EmaValues;
        }

        private async Task<IEnumerable<Candle>> GetCandlesAsync(Market market)
        {
            return await _exchangeService.GetCandlesAsync(market.Name, _tradingContext.Interval,
                end: _tradingContext.CurrentTime);
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
}
