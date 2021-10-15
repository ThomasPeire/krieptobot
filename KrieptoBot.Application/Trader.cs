using KrieptoBot.Application.Recommendators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public class Trader : ITrader
    {

        private readonly IExchangeService _exchangeService;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly ITradingContext _tradingContext;

        public Trader(IExchangeService exchangeService, IRecommendationCalculator recommendationCalculator, ITradingContext tradingContext)
        {
            _exchangeService = exchangeService;
            _recommendationCalculator = recommendationCalculator;
            _tradingContext = tradingContext;
        }

        public async Task Run()
        {
            var recommendations = await GetRecommendations();

            var marketsToSell = DetermineMarketsToSell(recommendations);
            var marketsToBuy = DetermineMarketsToBuy(recommendations);

            Sell(marketsToSell);

            var marketsToBuyWithBudget = GetMarketsToBuyWithBudget(marketsToBuy, recommendations);

            Buy(marketsToBuyWithBudget);
        }

        private static Dictionary<string, float> GetMarketsToBuyWithBudget(IDictionary<string, RecommendatorScore> marketsToBuy, Dictionary<string, RecommendatorScore> recommendations)
        {
            var availableBudgetToInvest = GetAvailableBudgetToInvest();

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

            return recommendations
                    .Where(x => marketsToBuy.Keys.Contains(x.Key))
                    .ToDictionary(
                        marketToBuyRecommendation => marketToBuyRecommendation.Key,
                        marketToBuyRecommendation => (float)Math.Floor(marketToBuyRecommendation.Value.Score *
                            availableBudgetToInvest / totalRecommendationScore));
        }

        private static float GetAvailableBudgetToInvest()
        {
            return 1000F; // todo get from api
        }

        private void Buy(Dictionary<string, float> marketsToBuyWithBudget)
        {
            foreach (var (market, budget) in marketsToBuyWithBudget)
            {
                Debug.WriteLine($"Buying {budget} on {market} ");
            }
        }

        private void Sell(IDictionary<string, RecommendatorScore> marketsToSell)
        {
            foreach (var (market, _) in marketsToSell)
            {
                // go to service => place order
                Debug.WriteLine($"Selling on {market} ");
            }
        }

        private IDictionary<string, RecommendatorScore> DetermineMarketsToBuy(Dictionary<string, RecommendatorScore> recommendations)
        {
            var marketsToBuy =
                recommendations
                    .Where(x => x.Value.Score >= _tradingContext.BuyMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            return marketsToBuy;
        }

        private IDictionary<string, RecommendatorScore> DetermineMarketsToSell(Dictionary<string, RecommendatorScore> recommendations)
        {
            var marketsToSell =
                recommendations
                    .Where(x => x.Value.Score <= _tradingContext.SellMargin)
                    .ToDictionary(x => x.Key, x => x.Value);

            return marketsToSell;
        }

        private async Task<Dictionary<string, RecommendatorScore>> GetRecommendations()
        {
            var marketsToEvaluate = GetMarketsToEvaluate();

            var marketRecommendations = await Task.WhenAll(marketsToEvaluate.Select(async market => (market, recommendation: await _recommendationCalculator.CalculateRecommendation(market))));

            return marketRecommendations.ToDictionary(x => x.market, x => x.recommendation);
        }


        private IEnumerable<string> GetMarketsToEvaluate()
        {
            var marketsToWatch = _tradingContext.MarketsToWatch;
            
            return marketsToWatch;
        }
        
    }
}
