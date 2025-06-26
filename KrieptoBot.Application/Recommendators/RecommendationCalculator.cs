using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators;

public class RecommendationCalculator(
    ILogger<RecommendationCalculator> logger,
    IRecommendatorSorter recommendatorSorter)
    : IRecommendationCalculator
{
    private readonly ILogger<RecommendationCalculator> _logger = logger;

    public async Task<RecommendatorScore> CalculateRecommendation(Market market)
    {
        var sortedRecommendators = recommendatorSorter.GetSortRecommendators().ToList();
        List<RecommendatorScore> recommendationScores = new();

        foreach (var recommendator in sortedRecommendators)
        {
            recommendationScores.Add(await recommendator.GetRecommendation(market));
        }

        var averageScore = recommendationScores.Where(x => x.IncludeInAverageScore).Sum(x => x);

        return new RecommendatorScore(averageScore);
    }
}