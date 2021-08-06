using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    public interface IRecommendator
    {
        Task<RecommendatorScore> GetRecommendation(string market);
    }
}
