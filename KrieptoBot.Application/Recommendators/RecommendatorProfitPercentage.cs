﻿using System;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorProfitPercentage : RecommendatorBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<RecommendatorProfitPercentage> _logger;

        public RecommendatorProfitPercentage(ILogger<RecommendatorProfitPercentage> logger,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        protected override decimal SellRecommendationWeight => 1; //todo make app setting for each recommendator
        protected override decimal BuyRecommendationWeight => 0; //todo make app setting for each recommendator

        protected override async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var recommendatorScore = 0m;
//todo this should use gettrades instead of orders
            var lastOrders =
                (await _exchangeService.GetOrdersAsync(market.Name, 20, end: DateTime.Now)).OrderBy(
                    x => x.Created).ToList();
            var lastBuyOrders = lastOrders
                .Skip(lastOrders.FindLastIndex(x => x.Side == OrderSide.Sell && x.Status != OrderStatus.Canceled) + 1)
                .Where(x => x.Status != OrderStatus.Canceled).ToList();

            if (lastBuyOrders.Any())
            {
                var averagePricePaid = lastBuyOrders.Sum(x => x.Price * x.Amount) / lastBuyOrders.Sum(x => x.Amount);

                var tickerPrice = await _exchangeService.GetTickerPrice(market.Name);

                var relativeProfitInPct = tickerPrice.Price / averagePricePaid * 100 - 100;

                _logger.LogInformation(
                    "Market {Market}: Current profit: {Profit}%",
                    market.Name, relativeProfitInPct);

                // negative score = sell recommendation
                // the larger the profit => the stronger the sell recommendation should be
                // no buy recommendation should be made when profit is negative => never a positive score
                recommendatorScore = Math.Min(-relativeProfitInPct, 0);
            }

            _logger.LogInformation(
                "Market {Market}: Profit recommendator gives recommendation score of {Score}",
                market.Name, recommendatorScore);

            return new RecommendatorScore(recommendatorScore, lastBuyOrders.Any());
        }
    }
}