using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public RecommendatorMacd(IOptions<RecommendatorSettings> recommendatorSettings, ILogger<RecommendatorMacd> logger, IMacd macd,
        IExchangeService exchangeService, ITradingContext tradingContext)
        : base(recommendatorSettings.Value, logger)
    {
        _logger = logger;
        _macd = macd;
        _exchangeService = exchangeService;
        _tradingContext = tradingContext;
    }

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var macdRecommendation = await GetMacdRecommendationValue(market);

        return new RecommendatorScore(macdRecommendation);
    }

    private async Task<decimal> GetMacdRecommendationValue(Market market)
    {
        var candles = await GetCandlesAsync(market);

        var macdValues = _macd.Calculate(candles);

        var lastValues = macdValues.OrderByDescending(x => x.Key).Skip(1).Take(2).ToList();
        var currentValue = lastValues[0].Value;
        var previousVal = lastValues[1].Value;

        _logger.LogInformation("Market {Market} - {Recommendator} Macd previous: {previousValue} - Macd current: {currentValue}",
            market.Name.Value, Name, previousVal.ToString("0.00"), currentValue.ToString("0.00"));

        var macdStrength = Math.Abs(currentValue) + Math.Abs(previousVal);
        if (MacdGivesSellSignal(currentValue, previousVal))
        {
            return macdStrength * -1;
        }

        if (MacdGivesBuySignal(currentValue, previousVal))
        {
            return macdStrength;
        }

        return 0;
    }

    private bool MacdGivesSellSignal(decimal currentValue, decimal previousVal)
    {
        return previousVal > 0 && currentValue < 0;
    }

    private static bool MacdGivesBuySignal(decimal currentValue, decimal previousVal)
    {
        return previousVal < 0 && currentValue > 0;
    }


    private async Task<IEnumerable<Candle>> GetCandlesAsync(Market market)
    {
        return await _exchangeService.GetCandlesAsync(market.Name, _tradingContext.Interval,
            100,
            end: _tradingContext.CurrentTime);
    }
}
