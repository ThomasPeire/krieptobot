namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorScore
    {
        public decimal Score { get; init; }

        public static RecommendatorScore operator *(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore { Score = recommendatorScore.Score * b };
        public static RecommendatorScore operator /(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore { Score = recommendatorScore.Score / b };
        public static RecommendatorScore operator +(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore { Score = recommendatorScore.Score + b };
        public static RecommendatorScore operator -(RecommendatorScore recommendatorScore, decimal b) =>
            new RecommendatorScore { Score = recommendatorScore.Score - b };
        public static RecommendatorScore operator *(RecommendatorScore recommendatorScoreA, RecommendatorScore recommendatorScoreB) =>
            new RecommendatorScore { Score = recommendatorScoreA.Score * recommendatorScoreB.Score };
        public static RecommendatorScore operator /(RecommendatorScore recommendatorScoreA, RecommendatorScore recommendatorScoreB) =>
            new RecommendatorScore { Score = recommendatorScoreA.Score / recommendatorScoreB.Score };
        public static RecommendatorScore operator +(RecommendatorScore recommendatorScoreA, RecommendatorScore recommendatorScoreB) =>
            new RecommendatorScore { Score = recommendatorScoreA.Score + recommendatorScoreB.Score };
        public static RecommendatorScore operator -(RecommendatorScore recommendatorScoreA, RecommendatorScore recommendatorScoreB) =>
            new RecommendatorScore { Score = recommendatorScoreA.Score - recommendatorScoreB.Score };
    }
}
