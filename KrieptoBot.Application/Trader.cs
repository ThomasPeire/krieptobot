using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application
{
    public class Trader : ITrader
    {
        private readonly IBuyManager _buyManager;
        private readonly TradingSettings _tradingSettings;
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<Trader> _logger;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly ISellManager _sellManager;
        private readonly ITradingContext _tradingContext;

        public Trader(ILogger<Trader> logger, ITradingContext tradingContext, IExchangeService exchangeService,
            IRecommendationCalculator recommendationCalculator, ISellManager sellManager, IBuyManager buyManager,
            IOptions<TradingSettings> tradingSettings)
        {
            _exchangeService = exchangeService;
            _recommendationCalculator = recommendationCalculator;
            _sellManager = sellManager;
            _buyManager = buyManager;
            _tradingSettings = tradingSettings.Value;
            _tradingContext = tradingContext;
            _logger = logger;
        }

        public async Task Run()
        {
            var bitvavoTime = await _exchangeService.GetTime();
            _logger.LogInformation("Exchange time: {BitvavoTime}, Local time: {Local}", bitvavoTime.ToLocalTime(), DateTime.Now);

            await CancelOpenOrders();

            var recommendations = await GetRecommendations();

            LogFinalScores(recommendations);

            var marketsToSell = await DetermineMarketsToSell(recommendations);
            var marketsToBuy = await DetermineMarketsToBuy(recommendations);

            if (marketsToSell.Any())
                await Sell(marketsToSell.Select(x => x.Key));

            if (marketsToBuy.Any()) await Buy(marketsToBuy);
        }

        private void LogFinalScores(Dictionary<Market, RecommendatorScore> recommendations)
        {
            foreach (var recommendatorScore in recommendations)
            {
                _logger.LogInformation("Market {Market}: Final score: {Score}", recommendatorScore.Key.Name.Value,
                    recommendatorScore.Value.Value.ToString("0.00"));
            }
        }

        private async Task CancelOpenOrders()
        {
            if (!_tradingContext.IsSimulation)
            {
                await _exchangeService.CancelOrders(string.Empty);
            }
        }

        private async Task<Dictionary<Market, Amount>> DetermineBudgetForMarket(
            IDictionary<Market, RecommendatorScore> marketsToBuy)
        {
            var totalRecommendationScore = marketsToBuy.Sum(x => x.Value);

            var dictionary = new Dictionary<Market, Amount>();

            var availableBudget = new Amount((await GetAvailableBudgetToInvest()) / 100 * 95);

            foreach (var (market, recommendation) in marketsToBuy)
            {
                var amount = Math.Floor(recommendation.Value * availableBudget.Value /
                                        totalRecommendationScore);

                if (amount < _tradingSettings.MinBuyBudgetPerCoin)
                {
                    continue;
                }

                dictionary.Add(
                    market,
                    new Amount(Math.Min(amount, _tradingSettings.MaxBuyBudgetPerCoin)));
            }

            return dictionary;
        }

        private async Task<Amount> GetAvailableBudgetToInvest()
        {
            var balance = await GetAvailableBalanceForAsset(new Symbol("EUR"));

            _logger.LogInformation("Available budget: {Budget}", balance.Value);

            return balance;
        }

        private async Task<Amount> GetAvailableBalanceForAsset(Symbol asset)
        {
            var availableBalance = await _exchangeService.GetBalanceAsync(asset);

            return availableBalance != null
                ? new Amount(Math.Max(availableBalance.Available.Value - availableBalance.InOrder.Value, 0))
                : Amount.Zero;
        }

        private async Task Buy(Dictionary<Market, Amount> marketsWithBudget)
        {
            foreach (var (market, budget) in marketsWithBudget)
            {
                if (await NoTradesInCoolDownPeriod(market))
                {
                    await _buyManager.Buy(market, budget);
                }
            }
        }

        private async Task<bool> NoTradesInCoolDownPeriod(Market market)
        {
            var trades = await _exchangeService.GetTradesAsync(market.Name, 5, end: _tradingContext.CurrentTime);

            var buyTradesInCoolDown = trades.Any(x =>
                x.Side == OrderSide.Buy &&
                x.Timestamp.AddMinutes(_tradingContext.BuyCoolDownPeriodInMinutes) >= _tradingContext.CurrentTime);

            return buyTradesInCoolDown == false;
        }

        private async Task Sell(IEnumerable<Market> marketsToSell)
        {
            foreach (var market in marketsToSell)
                await _sellManager.Sell(market);
        }

        private async Task<Dictionary<Market, Amount>> DetermineMarketsToBuy(
            Dictionary<Market, RecommendatorScore> marketRecommendations)
        {
            var marketsToBuy = GetMarketsWhereBuyRecommendationExceedsMargin(marketRecommendations);

            var budgetForMarkets = await DetermineBudgetForMarket(marketsToBuy);
            budgetForMarkets = GetMarketsWhereAvailableBalanceExceedsMinimumQuoteAmount(budgetForMarkets);

            foreach (var (market, _) in budgetForMarkets)
                _logger.LogInformation("Buy recommendation for {Market}, with a score of {Score}", market.Name.Value,
                    marketsToBuy.First(x => x.Key == market).Value.Value.ToString("0.00"));

            return budgetForMarkets;
        }

        private Dictionary<Market, RecommendatorScore> GetMarketsWhereBuyRecommendationExceedsMargin(
            Dictionary<Market, RecommendatorScore> recommendations)
        {
            return recommendations
                .Where(x => x.Value >= _tradingContext.BuyMargin)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<IDictionary<Market, RecommendatorScore>> DetermineMarketsToSell(
            Dictionary<Market, RecommendatorScore> recommendations)
        {
            var marketsToSell = GetMarketsWhereSellRecommendationExceedsMargin(recommendations);

            marketsToSell = await GetMarketsWhereAvailableBalanceExceedsMinimumBaseAmount(marketsToSell);

            foreach (var (market, score) in marketsToSell)
                _logger.LogInformation("Sell recommendation for {Market}, with a score of {Score}", market.Name.Value,
                    score.Value.ToString("0.00"));

            return marketsToSell;
        }

        private async Task<Dictionary<Market, RecommendatorScore>>
            GetMarketsWhereAvailableBalanceExceedsMinimumBaseAmount(
                Dictionary<Market, RecommendatorScore> marketsToSell)
        {
            var marketsToSellWithEnoughBalance = new Dictionary<Market, RecommendatorScore>();

            foreach (var (market, recommendatorScore) in marketsToSell)
            {
                var balance = await GetAvailableBalanceForAsset(market.Name.BaseSymbol);
                if (market.MinimumBaseAmount <= balance) marketsToSellWithEnoughBalance.Add(market, recommendatorScore);
            }

            return marketsToSellWithEnoughBalance;
        }

        private static Dictionary<Market, Amount> GetMarketsWhereAvailableBalanceExceedsMinimumQuoteAmount(
            Dictionary<Market, Amount> marketsWithBudget)
        {
            return marketsWithBudget
                .Where(marketWithBudget => marketWithBudget.Key.MinimumQuoteAmount <= marketWithBudget.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<Market, RecommendatorScore> GetMarketsWhereSellRecommendationExceedsMargin(
            Dictionary<Market, RecommendatorScore> recommendations)
        {
            return recommendations
                .Where(x => x.Value <= _tradingContext.SellMargin)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<Dictionary<Market, RecommendatorScore>> GetRecommendations()
        {
            var marketsToEvaluate = await GetMarketsToEvaluate();

            var marketRecommendations =
                await Task.WhenAll(
                    marketsToEvaluate.Select(
                        async market => (market,
                            recommendation: await _recommendationCalculator.CalculateRecommendation(market))));

            return marketRecommendations.ToDictionary(x => x.market, x => x.recommendation);
        }

        private async Task<IEnumerable<Market>> GetMarketsToEvaluate()
        {
            var marketsToWatch = new List<Market>();

            foreach (var market in _tradingContext.MarketsToWatch)
                marketsToWatch.Add(await _exchangeService.GetMarketAsync(market));

            _logger.LogInformation("Markets to evaluate:");

            foreach (var market in marketsToWatch)
                _logger.LogInformation("{Market}", market.Name.Value);

            return marketsToWatch;
        }
    }
}
