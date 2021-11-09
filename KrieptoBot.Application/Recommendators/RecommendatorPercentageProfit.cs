using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Model;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorPercentageProfit : RecommendatorBase
    {
        private readonly ILogger<RecommendatorPercentageProfit> _logger;
        private readonly IExchangeService _exchangeService;
        protected override decimal SellRecommendationWeight => 3; //todo make app setting for each recommendator
        protected override decimal BuyRecommendationWeight => 0; //todo make app setting for each recommendator

        public RecommendatorPercentageProfit(ILogger<RecommendatorPercentageProfit> logger,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var recommendatorScore = 0m;

            var lastOrders =
                (await _exchangeService.GetOrdersAsync(market.MarketName, 20, end: DateTime.Now)).OrderBy(
                    x => x.Created).ToList();
            var lastBuyOrders = lastOrders
                .Skip(lastOrders.FindLastIndex(x => x.Side == "sell" && x.Status != "canceled") + 1)
                .Where(x => x.Status != "canceled").ToList();

            if (lastBuyOrders.Any())
            {
                var averagePricePaid = lastBuyOrders.Average(x => x.Price);

                var tickerPrice = await _exchangeService.GetTickerPrice(market.MarketName);

                var relativeProfitInPct = (tickerPrice.Price / averagePricePaid * 100) - 100;

                _logger.LogInformation(
                    "Market {Market}: Current profit: {Profit}%",
                    market.MarketName, relativeProfitInPct);

                // negative score = sell recommendation
                // the larger the profit => the stronger the sell recommendation should be
                // no buy recommendation should be made when profit is negative => never a positive score
                recommendatorScore = Math.Min(-relativeProfitInPct, 0);
            }

            _logger.LogInformation(
                "Market {Market}: Profit recommendator gives recommendation score of {Score}",
                market.MarketName, recommendatorScore);

            return new RecommendatorScore { Score = recommendatorScore };
        }
    }
}
