using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    public abstract class RecommendatorBase : IRecommendator
    {
        public virtual float Weight => 1F;

        protected abstract Task<RecommendatorScore> CalculateRecommendation(string market);

        public async Task<RecommendatorScore> GetRecommendation(string market)
        {
            return await CalculateRecommendation(market).ConfigureAwait(false) * Weight;
        }
    }
}
