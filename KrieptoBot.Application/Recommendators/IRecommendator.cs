using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application.Recommendators
{
    public interface IRecommendator
    {
        Task<RecommendatorScore> GetRecommendation(Market market);
    }
}
