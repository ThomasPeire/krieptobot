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

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorMacd : RecommendatorBase
{
    protected override string Name => "Macd recommendator";
    private readonly ILogger<RecommendatorMacd> _logger;
    private readonly IMacd _macd;
    private readonly IExchangeService _exchangeService;
    private readonly ITradingContext _tradingContext;
    private readonly IExponentialMovingAverage _ema;

    public RecommendatorMacd(IOptions<RecommendatorSettings> recommendatorSettings, ILogger<RecommendatorMacd> logger,
        IMacd macd,
        IExchangeService exchangeService, ITradingContext tradingContext, IExponentialMovingAverage ema)
        : base(recommendatorSettings.Value, logger)
    {
        _logger = logger;
        _macd = macd;
        _exchangeService = exchangeService;
        _tradingContext = tradingContext;
        _ema = ema;
    }

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var macdRecommendation = await GetMacdRecommendationValue(market);

        return new RecommendatorScore(macdRecommendation);
    }

    private async Task<decimal> GetMacdRecommendationValue(Market market)
    {
        var candles = (await GetCandlesAsync(market)).ToList();

        var macdResult = _macd.Calculate(candles);

        var lastHistogramValues = macdResult.Histogram.OrderByDescending(x => x.Key).Take(3).ToList();
        var currentValue = lastHistogramValues[0].Value;
        var previousVal = lastHistogramValues[1].Value;
        var currentMacdLineValue = macdResult.MacdLine.OrderByDescending(x => x.Key).FirstOrDefault().Value;

        _logger.LogDebug(
            "Market {Market} - {Recommendator} CurrentMacd: {MacdValue}, Histogram (previous, current): {PreviousValue}, {CurrentValue}",
            market.Name.Value, Name, currentMacdLineValue.ToString("0.0000000000"), previousVal.ToString("0.0000000000"),
            currentValue.ToString("0.0000000000"));
        
        if (MacdGivesSellSignal(lastHistogramValues) && currentMacdLineValue > 0)
        {
            return RecommendationAction.Sell;
        }

        if (MacdGivesBuySignal(currentValue, previousVal) && currentMacdLineValue < 0)
        {
            return RecommendationAction.Buy;
        }

        return 0;
    }

    private bool MacdGivesSellSignal(List<KeyValuePair<DateTime, decimal>> lastHistogramValues)
    {
        const int numberOfConsecutiveDescendingValues = 2;
        var values = lastHistogramValues.OrderByDescending(x => x.Key).ToList();

        for (var i = 0; i < numberOfConsecutiveDescendingValues; i++)
        {
            if (values[i].Value > values[i + 1].Value)
            {
                return false;
            }
        }

        return true;
    }

    private static bool MacdGivesBuySignal(decimal currentValue, decimal previousVal)
    {
        return previousVal < 0 && currentValue > 0;
    }


    private async Task<IEnumerable<Candle>> GetCandlesAsync(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name, _tradingContext.Interval,
            end: _tradingContext.CurrentTime);
    }
}
