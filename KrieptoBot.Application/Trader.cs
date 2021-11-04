using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class Trader : ITrader
    {
        private readonly IExchangeService _exchangeService;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly ISellManager _sellManager;
        private readonly IBuyManager _buyManager;
        private readonly ITradingContext _tradingContext;
        private readonly ILogger<Trader> _logger;

        public Trader(ILogger<Trader> logger, ITradingContext tradingContext, IExchangeService exchangeService,
            IRecommendationCalculator recommendationCalculator, ISellManager sellManager, IBuyManager buyManager
        )
        {
            _exchangeService = exchangeService;
            _recommendationCalculator = recommendationCalculator;
            _sellManager = sellManager;
            _buyManager = buyManager;
            _tradingContext = tradingContext;
            _logger = logger;
        }

        public async Task Run()
        {
            var recommendations = await GetRecommendations();

            var marketsToSell = DetermineMarketsToSell(recommendations);
            var marketsToBuy = DetermineMarketsToBuy(recommendations);

            await Sell(marketsToSell);

            var marketsToBuyWithBudget = await GetMarketsToBuyWithBudget(marketsToBuy, recommendations);

            await Buy(marketsToBuyWithBudget);
        }

        private async Task<Dictionary<string, float>> GetMarketsToBuyWithBudget(
            IDictionary<string, RecommendatorScore> marketsToBuy,
            Dictionary<string, RecommendatorScore> recommendations)
        {
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
            var balance = await GetAvailableBalanceForAsset("EUR");

            _logger.LogInformation("Available budget: {Budget}", balance);

            return balance;
        }

        private async Task<float> GetAvailableBalanceForAsset(string asset)
        {
            var balances = await _exchangeService.GetBalanceAsync();

            var availableBalance = balances.FirstOrDefault(x => x.Symbol.Equals(asset));

            return availableBalance != null
                ? (float)(availableBalance.Available - availableBalance.InOrder)
                : 0f;
        }

        private async Task Buy(Dictionary<string, float> marketsToBuyWithBudget)
        {
            foreach (var (market, budget) in marketsToBuyWithBudget)
            {
                await _buyManager.Buy(market, budget);
            }
        }

        private async Task Sell(IDictionary<string, RecommendatorScore> marketsToSell)
        {
            foreach (var (market, _) in marketsToSell)
            {
                await _sellManager.Sell(market);
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
