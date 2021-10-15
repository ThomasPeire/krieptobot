using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        public virtual float SellRecommendationWeight => 1F;
        public virtual float BuyRecommendationWeight => 1F;

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
