using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        public override float BuyRecommendationWeight => 0.1F;
        public override float SellRecommendationWeight => 0.1F;

        protected override Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore { Score = .0F });
        }
    }
}
