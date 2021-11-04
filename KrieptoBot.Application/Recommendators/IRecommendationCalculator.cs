using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application.Recommendators
{
    public interface IRecommendationCalculator
    {
        Task<RecommendatorScore> CalculateRecommendation(Market market);
    }
}
