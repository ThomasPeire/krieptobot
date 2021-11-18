using System;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
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
            recommendatorSettings.Value)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }


        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var recommendatorScore = 0m;

            var trades =
                (await _exchangeService.GetTradesAsync(market.Name, 20, end: DateTime.Now)).OrderBy(
                    x => x.Timestamp).ToList();
            var lastBuyTrades = trades
                .Skip(trades.FindLastIndex(x => x.Side == OrderSide.Sell) + 1).ToList();

            if (lastBuyTrades.Any())
            {
                var averagePricePaid = lastBuyTrades.Sum(x => x.Price * x.Amount) / lastBuyTrades.Sum(x => x.Amount);

                var tickerPrice = await _exchangeService.GetTickerPrice(market.Name);

                var relativeProfitInPct = tickerPrice.Price / averagePricePaid * 100 - 100;

                _logger.LogInformation(
                    "Market {Market}: Current profit: {Profit}%",
                    market.Name, relativeProfitInPct.ToString("0.00"));

                // negative score = sell recommendation
                // the larger the profit => the stronger the sell recommendation should be
                // no buy recommendation should be made when profit is negative => never a positive score
                recommendatorScore = Math.Min(-relativeProfitInPct, 0);
            }

            _logger.LogInformation(
                "Market {Market}: Profit recommendator gives recommendation score of {Score}",
                market.Name, recommendatorScore.ToString("0.00"));

            return new RecommendatorScore(recommendatorScore, lastBuyTrades.Any());
        }
    }
}
