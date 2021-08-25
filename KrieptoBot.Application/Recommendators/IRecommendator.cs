using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public interface IRecommendator
    {
        Task<RecommendatorScore> GetRecommendation(string market);
    }
}
