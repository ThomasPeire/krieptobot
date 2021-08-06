namespace KrieptoBod.Application.Recommendators
{
    public interface IRecommendationCalculator
    {
        RecommendatorScore CalculateRecommendation(string market);
    }
}
