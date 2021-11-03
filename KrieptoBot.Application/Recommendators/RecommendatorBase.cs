using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        protected virtual float SellRecommendationWeight => 1F; //todo make app setting for each recommendator
        protected virtual float BuyRecommendationWeight => 1F; //todo make app setting for each recommendator

        protected abstract Task<RecommendatorScore> CalculateRecommendation(string market);

        public async Task<RecommendatorScore> GetRecommendation(string market)
        {
            var recommendation = await CalculateRecommendation(market);
            return recommendation.Score > 0 ?
                recommendation * BuyRecommendationWeight :
                recommendation * SellRecommendationWeight;
        }
    }
}
