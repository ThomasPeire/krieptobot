using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        protected virtual decimal SellRecommendationWeight => 1; //todo make app setting for each recommendator
        protected virtual decimal BuyRecommendationWeight => 1; //todo make app setting for each recommendator

        protected abstract Task<RecommendatorScore> CalculateRecommendation(Market market);

        public async Task<RecommendatorScore> GetRecommendation(Market market)
        {
            var recommendation = await CalculateRecommendation(market);
            return recommendation.Score > 0
                ? recommendation * BuyRecommendationWeight
                : recommendation * SellRecommendationWeight;
        }
    }
}
