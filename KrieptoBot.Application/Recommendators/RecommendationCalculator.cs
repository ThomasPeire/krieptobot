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
        private readonly IRecommendatorSorter _recommendatorSorter;

        public RecommendationCalculator(ILogger<RecommendationCalculator> logger,
            IRecommendatorSorter recommendatorSorter)
        {
            _logger = logger;
            _recommendatorSorter = recommendatorSorter;
        }

        public async Task<RecommendatorScore> CalculateRecommendation(Market market)
        {
            var sortedRecommendators = _recommendatorSorter.GetSortRecommendators().ToList();
            List<RecommendatorScore> recommendationScores = new();

            foreach (var recommendator in sortedRecommendators)
            {
                recommendationScores.Add(await recommendator.GetRecommendation(market));
            }

            var averageScore = recommendationScores.Where(x => x.IncludeInAverageScore).Sum(x => x);

            _logger.LogInformation("Market {Market}: Final score: {Score}", market.Name.Value,
                averageScore.ToString("0.00"));

            return new RecommendatorScore(averageScore);
        }
    }
}
