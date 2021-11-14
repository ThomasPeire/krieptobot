using System.Threading.Tasks;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Recommendators
{
    public interface IRecommendator
    {
        Task<RecommendatorScore> GetRecommendation(Market market);
    }
}