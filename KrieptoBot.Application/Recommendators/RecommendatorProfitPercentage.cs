using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Constants;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorProfitPercentage : RecommendatorBase
{
    private readonly IExchangeService _exchangeService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<RecommendatorProfitPercentage> _logger;

    public RecommendatorProfitPercentage(ILogger<RecommendatorProfitPercentage> logger,
        IExchangeService exchangeService, IOptions<RecommendatorSettings> recommendatorSettings, IDateTimeProvider dateTimeProvider) : base(
        recommendatorSettings.Value, logger)
    {
        _logger = logger;
        _exchangeService = exchangeService;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override string Name => "Profit recommendator";

    protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var recommendatorScore = RecommendationAction.None;

        //todo figure out a way to calculate profit when last trade was only a partially filled sell order

        var trades =
            (await _exchangeService.GetTradesAsync(market.Name, 50, end: await _dateTimeProvider.UtcDateTimeNowSyncedWithExchange())).OrderBy(
                x => x.Timestamp).ToList();
        var lastBuyTrades = trades
            .Skip(trades.FindLastIndex(x => x.Side == OrderSide.Sell) + 1).ToList();

        if (!lastBuyTrades.Any())
            return new RecommendatorScore(RecommendationAction.None, false);

        var averagePricePaid = GetAveragePricePaid(lastBuyTrades);
        var priceToCompare = await GetPriceToCompare(market);
        var relativeProfitInPct = CalculateRelativeProfitInPct(priceToCompare, averagePricePaid);

        LogCurrentProfit(market, relativeProfitInPct);

        if (relativeProfitInPct >= RecommendatorSettings.ProfitRecommendatorProfitThresholdPct)
        {
            recommendatorScore = RecommendationAction.Sell;
        }
        if (relativeProfitInPct <= -RecommendatorSettings.ProfitRecommendatorLossThresholdPct)
        {
            recommendatorScore = RecommendationAction.Sell;
        }

        return new RecommendatorScore(recommendatorScore);
    }


    private void LogCurrentProfit(Market market, decimal relativeProfitInPct)
    {
        _logger.LogDebug(
            "Market {Market} - {Recommendator} Profit of: {Profit}%",
            market.Name.Value, Name, relativeProfitInPct.ToString("0.00"));
    }

    private static decimal CalculateRelativeProfitInPct(TickerPrice priceToCompare, decimal averagePricePaid)
    {
        return priceToCompare.Price / averagePricePaid * 100 - 100;
    }

    private async Task<TickerPrice> GetPriceToCompare(Market market)
    {
        return await _exchangeService.GetTickerPrice(market.Name);
    }

    private static decimal GetAveragePricePaid(IReadOnlyCollection<Trade> lastBuyTrades)
    {
        return lastBuyTrades.Sum(x => x.Price * x.Amount) / lastBuyTrades.Sum(x => x.Amount);
    }
}