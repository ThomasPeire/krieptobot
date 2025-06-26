using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Recommendation.ValueObjects;

public class RecommendatorScore(decimal value, bool includeInAverageScore = true) : ValueObject
{
    public bool IncludeInAverageScore { get; } = includeInAverageScore;
    public decimal Value { get; } = value;

    public static implicit operator decimal(RecommendatorScore score)
    {
        return score.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return IncludeInAverageScore;
    }

    public static RecommendatorScore operator *(RecommendatorScore recommendatorScore, decimal b)
    {
        return new RecommendatorScore(recommendatorScore.Value * b, recommendatorScore.IncludeInAverageScore);
    }

    public static RecommendatorScore operator /(RecommendatorScore recommendatorScore, decimal b)
    {
        return new RecommendatorScore(recommendatorScore.Value / b, recommendatorScore.IncludeInAverageScore);
    }

    public static RecommendatorScore operator +(RecommendatorScore recommendatorScore, decimal b)
    {
        return new RecommendatorScore(recommendatorScore.Value + b, recommendatorScore.IncludeInAverageScore);
    }

    public static RecommendatorScore operator -(RecommendatorScore recommendatorScore, decimal b)
    {
        return new RecommendatorScore(recommendatorScore.Value - b, recommendatorScore.IncludeInAverageScore);
    }
}