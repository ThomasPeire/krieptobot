using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    //take weighted average of recommendations (eg. one recommendation can be more important than others)
    public class RecommendationCalculator: IRecommendationCalculator
    {
        private readonly IEnumerable<IRecommendator> _recommendators;
        public RecommendationCalculator(IEnumerable<IRecommendator> recommendators)
        {
            _recommendators = recommendators;
        }
        
        public async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var recommendationScores = await Task.WhenAll(_recommendators.Select(async recommendator => await recommendator.GetRecommendation(market))) ;
            /* Example:
             * SELL -60
             * BUY 52
             * SELL -10
             * SELL -15
             * BUY 80
             *
             * OUTCOME
             * Sum SELL scores - SUM BUY SCORES: (-60 + -10 + -15 = -85) + (52 + 80 = 132) = 47
             * ==> positive score ==> ACTION = BUY with score 47
             */

            return new RecommendatorScore() { Score = recommendationScores.Sum(x => x.Score) };
        }
    }
}
