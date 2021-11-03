using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        protected override float BuyRecommendationWeight => 0.1F;
        protected override float SellRecommendationWeight => 0.1F;

        protected override Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore { Score = .0F });
        }
    }
}
