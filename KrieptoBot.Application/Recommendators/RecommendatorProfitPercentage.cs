using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorProfitPercentage : RecommendatorBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<RecommendatorProfitPercentage> _logger;

        public RecommendatorProfitPercentage(ILogger<RecommendatorProfitPercentage> logger,
            IExchangeService exchangeService, IOptions<RecommendatorSettings> recommendatorSettings) : base(
            recommendatorSettings.Value, logger)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        protected override string Name => "Profit recommendator";

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var recommendatorScore = 0m;

            var trades =
                (await _exchangeService.GetTradesAsync(market.Name, 20, end: DateTime.Now)).OrderBy(
                    x => x.Timestamp).ToList();
            var lastBuyTrades = trades
                .Skip(trades.FindLastIndex(x => x.Side == OrderSide.Sell) + 1).ToList();

            if (!lastBuyTrades.Any())
                return new RecommendatorScore(recommendatorScore, lastBuyTrades.Any());

            var averagePricePaid = GetAveragePricePaid(lastBuyTrades);
            var priceToCompare = await GetPriceToCompare(market);
            var relativeProfitInPct = CalculateRelativeProfitInPct(priceToCompare, averagePricePaid);

            LogCurrentProfit(market, relativeProfitInPct);

            // negative score = sell recommendation
            // the larger the profit => the stronger the sell recommendation should be
            // no buy recommendation should be made when profit is negative => never a positive score
            recommendatorScore = Math.Min(-relativeProfitInPct, 0);

            return new RecommendatorScore(recommendatorScore, lastBuyTrades.Any());
        }


        private void LogCurrentProfit(Market market, decimal relativeProfitInPct)
        {
            _logger.LogInformation(
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
}
