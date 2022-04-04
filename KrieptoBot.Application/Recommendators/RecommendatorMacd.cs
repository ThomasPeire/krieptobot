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

public class RecommendatorMacd : RecommendatorBase
{
    protected override string Name => "Macd recommendator";
    private readonly ILogger<RecommendatorMacd> _logger;
    private readonly IMacd _macd;
    private readonly IExchangeService _exchangeService;
    private readonly ITradingContext _tradingContext;

    public RecommendatorMacd(IOptions<RecommendatorSettings> recommendatorSettings, ILogger<RecommendatorMacd> logger,
        IMacd macd,
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

        var lastValues = macdValues.OrderByDescending(x => x.Key).Take(3).ToList();
        var currentValue = lastValues[0].Value;
        var previousVal = lastValues[1].Value;
        var previousPreviousVal = lastValues[2].Value;


        _logger.LogDebug(
            "Market {Market} - {Recommendator} Macd values: {PreviousPreviousValue}, {PreviousValue}, {CurrentValue}",
            market.Name.Value, Name, previousPreviousVal.ToString("0.0000000000"), previousVal.ToString("0.0000000000"),
            currentValue.ToString("0.0000000000"));

        if (MacdGivesSellSignal(currentValue, previousVal))
        {
            return RecommendationAction.Sell;
        }

        if (MacdGivesBuySignal(currentValue, previousVal))
        {
            return RecommendationAction.Buy;
        }

        return 0;
    }

    private static bool MacdGivesSellSignal(decimal currentValue, decimal previousVal)
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
