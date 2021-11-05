using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Model;
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

            var marketsToBuyWithBudget = await GetMarketsToBuyWithBudget(marketsToBuy);

            await Buy(marketsToBuyWithBudget);
        }

        private async Task<Dictionary<Market, decimal>> GetMarketsToBuyWithBudget(
            IDictionary<Market, RecommendatorScore> marketsToBuy)
        {
            var totalRecommendationScore = marketsToBuy.Sum(x => x.Value.Score);

            var dictionary = new Dictionary<Market, decimal>();

            foreach (var (market, recommendation) in marketsToBuy)
            {
                dictionary.Add(
                    market,
                    Math.Floor(recommendation.Score * await GetAvailableBudgetToInvest() /
                                      totalRecommendationScore));
            }

            return dictionary;
        }

        private async Task<decimal> GetAvailableBudgetToInvest()
        {
            var balance = await GetAvailableBalanceForAsset("EUR");

            _logger.LogInformation("Available budget: {Budget}", balance);

            return balance;
        }

        private async Task<decimal> GetAvailableBalanceForAsset(string asset)
        {
            var balances = await _exchangeService.GetBalanceAsync();

            var availableBalance = balances.FirstOrDefault(x => x.Symbol.Equals(asset));

            return availableBalance != null
                ? (availableBalance.Available - availableBalance.InOrder)
                : 0;
        }

        private async Task Buy(Dictionary<Market, decimal> marketsToBuyWithBudget)
        {
            foreach (var (market, budget) in marketsToBuyWithBudget)
            {
                await _buyManager.Buy(market, budget);
            }
        }

        private async Task Sell(IDictionary<Market, RecommendatorScore> marketsToSell)
        {
            foreach (var (market, _) in marketsToSell)
            {
                await _sellManager.Sell(market);
            }
        }

        private IDictionary<Market, RecommendatorScore> DetermineMarketsToBuy(
            Dictionary<Market, RecommendatorScore> recommendations)
        {
            var marketsToBuy =
                recommendations
                    .Where(x => x.Value.Score >= _tradingContext.BuyMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach (var (market, score) in marketsToBuy)
            {
                _logger.LogInformation("Buy recommendation for {Market}, with a score of {Score}", market.MarketName, score.Score);
            }

            return marketsToBuy;
        }

        private IDictionary<Market, RecommendatorScore> DetermineMarketsToSell(
            Dictionary<Market, RecommendatorScore> recommendations)
        {
            var marketsToSell =
                recommendations
                    .Where(x => x.Value.Score <= _tradingContext.SellMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            foreach (var (market, score) in marketsToSell)
            {
                _logger.LogInformation("Sell recommendation for {Market}, with a score of {Score}", market.MarketName,
                    score.Score);
            }

            return marketsToSell;
        }

        private async Task<Dictionary<Market, RecommendatorScore>> GetRecommendations()
        {
            var marketsToEvaluate = await GetMarketsToEvaluate();

            var marketRecommendations = await Task.WhenAll(marketsToEvaluate. Select(async market =>
                (market, recommendation: await _recommendationCalculator.CalculateRecommendation(market))));

            return marketRecommendations.ToDictionary(x => x.market, x => x.recommendation);
        }

        private async Task<IEnumerable<Market>> GetMarketsToEvaluate()
        {
            var marketsToWatch = new List<Market>();

            foreach (var market in _tradingContext.MarketsToWatch)
            {
                marketsToWatch.Add(await _exchangeService.GetMarketAsync(market));
            }

            _logger.LogInformation("Markets to evaluate:");

            foreach (var market in marketsToWatch)
            {
                _logger.LogInformation("{Market}", market.MarketName);
            }

            return marketsToWatch;
        }
    }
}
