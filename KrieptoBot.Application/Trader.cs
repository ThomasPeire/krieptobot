using KrieptoBot.Application.Recommendators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Model;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class Trader : ITrader
    {
        private readonly IExchangeService _exchangeService;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly ITradingContext _tradingContext;
        private readonly ILogger<Trader> _logger;

        public Trader(IExchangeService exchangeService, IRecommendationCalculator recommendationCalculator,
            ITradingContext tradingContext, ILogger<Trader> logger)
        {
            _exchangeService = exchangeService;
            _recommendationCalculator = recommendationCalculator;
            _tradingContext = tradingContext;
            _logger = logger;
        }

        public async Task Run()
        {
            var recommendations = await GetRecommendations();

            var marketsToSell = DetermineMarketsToSell(recommendations);
            var marketsToBuy = DetermineMarketsToBuy(recommendations);

            Sell(marketsToSell);

            var marketsToBuyWithBudget = await GetMarketsToBuyWithBudget(marketsToBuy, recommendations);

            Buy(marketsToBuyWithBudget);
        }

        private async Task<Dictionary<string, float>> GetMarketsToBuyWithBudget(
            IDictionary<string, RecommendatorScore> marketsToBuy,
            Dictionary<string, RecommendatorScore> recommendations)
        {
            var availableBudgetToInvest = await GetAvailableBudgetToInvest();

            /*
             * example
             * budgetToInvest = 999
             * btc => score 50
             * ada => score 40
             * chz => score 90
             * ltc => score 500
             * => sum of scores = 50+40+90+500 = 680
             * ==> budgets for coins
             * btc => 50 * 999/680 => math.floor 73.4
             * ada => 40 * 999/680 => math.floor 58.7
             * chz => 90 * 999/680 => math.floor 132.2
             * ltc => 500 * 999/680 => math.floor 734.5
             */
            var totalRecommendationScore = marketsToBuy.Sum(x => x.Value.Score);

            var dictionary = new Dictionary<string, float>();

            foreach (var (market, recommendation) in recommendations.Where(x => marketsToBuy.ContainsKey(x.Key)))
            {
                dictionary.Add(
                    market,
                    (float)Math.Floor(recommendation.Score * await GetAvailableBudgetToInvest() /
                                      totalRecommendationScore));
            }

            return dictionary;
        }

        private async Task<float> GetAvailableBudgetToInvest()
        {
            var balances = await _exchangeService.GetBalanceAsync();

            var availableBuyBalance = balances.FirstOrDefault(x => x.Symbol.Equals("EUR"));

            if (availableBuyBalance != null)
            {
                _logger.LogInformation("Available budget: {Budget}",
                    availableBuyBalance.Available - availableBuyBalance.InOrder);
            }

            return availableBuyBalance != null
                ? (float)(availableBuyBalance.Available - availableBuyBalance.InOrder)
                : 0f;
        }

        private void Buy(Dictionary<string, float> marketsToBuyWithBudget)
        {
            foreach (var (market, budget) in marketsToBuyWithBudget)
            {
                _logger.LogDebug("Buying on {Market} with budget {Budget}", market, budget);
            }
        }

        private void Sell(IDictionary<string, RecommendatorScore> marketsToSell)
        {
            foreach (var (market, _) in marketsToSell)
            {
                // go to service => place order
                _logger.LogDebug("Selling on {Market}", market);
            }
        }

        private IDictionary<string, RecommendatorScore> DetermineMarketsToBuy(
            Dictionary<string, RecommendatorScore> recommendations)
        {
            var marketsToBuy =
                recommendations
                    .Where(x => x.Value.Score >= _tradingContext.BuyMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach (var (market, score) in marketsToBuy)
            {
                _logger.LogInformation("Buy recommendation for {Market}, with a score of {Score}", market, score.Score);
            }

            return marketsToBuy;
        }

        private IDictionary<string, RecommendatorScore> DetermineMarketsToSell(
            Dictionary<string, RecommendatorScore> recommendations)
        {
            var marketsToSell =
                recommendations
                    .Where(x => x.Value.Score <= _tradingContext.SellMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach (var (market, score) in marketsToSell)
            {
                _logger.LogInformation("Sell recommendation for {Market}, with a score of {Score}", market,
                    score.Score);
            }

            return marketsToSell;
        }

        private async Task<Dictionary<string, RecommendatorScore>> GetRecommendations()
        {
            var marketsToEvaluate = GetMarketsToEvaluate();

            var marketRecommendations = await Task.WhenAll(marketsToEvaluate.Select(async market =>
                (market, recommendation: await _recommendationCalculator.CalculateRecommendation(market))));

            return marketRecommendations.ToDictionary(x => x.market, x => x.recommendation);
        }


        private IEnumerable<string> GetMarketsToEvaluate()
        {
            var marketsToWatch = _tradingContext.MarketsToWatch.ToList();

            _logger.LogInformation("Markets to evaluate:");

            foreach (var market in marketsToWatch)
            {
                _logger.LogInformation("{Market}", market);
            }

            return marketsToWatch;
        }
    }
}
