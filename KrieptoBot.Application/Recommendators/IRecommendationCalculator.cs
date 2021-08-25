using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public interface IRecommendationCalculator
    {
        Task<RecommendatorScore> CalculateRecommendation(string market);
    }
}
