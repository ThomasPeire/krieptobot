using System.Threading.Tasks;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorSupport : RecommendatorBase
    {
        public RecommendatorSupport(IOptions<RecommendatorSettings> recommendatorSettings) : base(recommendatorSettings
            .Value)
        {
        }

        protected override Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            //To be implemented
            return Task.FromResult(new RecommendatorScore(.0m, false));
        }
    }
}