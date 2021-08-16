using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    public interface IRecommendationCalculator
    {
        Task<RecommendatorScore> CalculateRecommendation(string market);
    }
}
