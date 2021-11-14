using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        protected override decimal BuyRecommendationWeight => 0.1m;
        protected override decimal SellRecommendationWeight => 0.1m;

        protected override Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore(.0m, false));
        }
    }
}