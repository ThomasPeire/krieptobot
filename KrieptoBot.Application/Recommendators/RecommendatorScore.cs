namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorScore
    {
        public bool IncludeInAverageScore { get; init; } = true;
        public decimal Score { get; init; }

        public static RecommendatorScore operator *(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore
            {
                Score = recommendatorScore.Score * b, IncludeInAverageScore = recommendatorScore.IncludeInAverageScore
            };

        public static RecommendatorScore operator /(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore
            {
                Score = recommendatorScore.Score / b, IncludeInAverageScore = recommendatorScore.IncludeInAverageScore
            };

        public static RecommendatorScore operator +(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore
            {
                Score = recommendatorScore.Score + b, IncludeInAverageScore = recommendatorScore.IncludeInAverageScore
            };

        public static RecommendatorScore operator -(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore
            {
                Score = recommendatorScore.Score - b, IncludeInAverageScore = recommendatorScore.IncludeInAverageScore
            };
    }
}
