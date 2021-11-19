using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendationCalculator : IRecommendationCalculator
    {
        private readonly ILogger<RecommendationCalculator> _logger;
        private readonly IEnumerable<IRecommendator> _recommendators;

        public RecommendationCalculator(ILogger<RecommendationCalculator> logger,
            IEnumerable<IRecommendator> recommendators)
        {
            _logger = logger;
            _recommendators = recommendators;
        }

        public async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var recommendationScores =
                await Task.WhenAll(_recommendators.Select(async recommendator =>
                    await recommendator.GetRecommendation(market)));

            var averageScore = recommendationScores.Where(x => x.IncludeInAverageScore).Average(x => x);

            _logger.LogInformation("Market {Market}: Final score: {Score}", market.Name.Value,
                averageScore.ToString("0.00"));

            return new RecommendatorScore(averageScore);
        }
    }
}