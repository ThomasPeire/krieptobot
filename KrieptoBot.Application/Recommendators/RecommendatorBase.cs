using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        private readonly RecommendatorSettings _recommendatorSettings;

        protected RecommendatorBase(RecommendatorSettings recommendatorSettings)
        {
            _recommendatorSettings = recommendatorSettings;
        }

        private decimal SellRecommendationWeight =>
            _recommendatorSettings.SellRecommendationWeights.GetValueOrDefault(GetType().Name);

        private decimal BuyRecommendationWeight =>
            _recommendatorSettings.BuyRecommendationWeights.GetValueOrDefault(GetType().Name);

        public async Task<RecommendatorScore> GetRecommendation(Market market)
        {
            var recommendation = await CalculateRecommendation(market);
            return recommendation > 0
                ? recommendation * BuyRecommendationWeight
                : recommendation * SellRecommendationWeight;
        }

        protected abstract Task<RecommendatorScore> CalculateRecommendation(Market market);
    }
}
