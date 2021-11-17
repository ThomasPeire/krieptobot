﻿using System.Linq;
using System.Threading.Tasks;
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
            int periodToAverage, RecommendatorSettings recommendatorSettings) : base(recommendatorSettings)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
            _tradingContext = tradingContext;
            _logger = logger;
            _tradingContextInterval = tradingContextInterval;
            _periodToAverage = periodToAverage;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market.Name, _tradingContextInterval,
                _periodToAverage * 10,
                end: _tradingContext.CurrentTime);

            var rsiValues = _rsiIndicator.Calculate(candles, _periodToAverage);

            var (_, value) = rsiValues.OrderBy(x => x.Key).Last();

            _logger.LogInformation("Market {Market}: RSI{PeriodToAverage} on interval {Interval} is {RsiValue}",
                market.Name, _periodToAverage, _tradingContextInterval, value);

            var recommendatorScore = EvaluateRsiValue(value);

            _logger.LogInformation(
                "Market {Market}: RSI{PeriodToAverage} on interval {Interval} gives recommendation score of {Score}",
                market.Name, _periodToAverage, _tradingContextInterval, recommendatorScore.Value);

            return recommendatorScore;
        }

        private static RecommendatorScore EvaluateRsiValue(decimal rsiValue)
        {
            var rsiRecommendation = new RecommendatorScore(100 - rsiValue * 2);

            return rsiRecommendation;
        }
    }
}
