using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        protected override decimal BuyRecommendationWeight => 0.1m;
        protected override decimal SellRecommendationWeight => 0.1m;

        protected override Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore { Score = .0m });
        }
    }
}
