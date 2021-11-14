using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        protected virtual decimal SellRecommendationWeight => 1; //todo make app setting for each recommendator
        protected virtual decimal BuyRecommendationWeight => 1; //todo make app setting for each recommendator

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