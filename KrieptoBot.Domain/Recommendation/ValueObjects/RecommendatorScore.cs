using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Recommendation.ValueObjects;

public class RecommendatorScore : ValueObject
{
    public RecommendatorScore(decimal value, bool includeInAverageScore = true)
    {
        Value = value;
        IncludeInAverageScore = includeInAverageScore;
    }

    public bool IncludeInAverageScore { get; }
    public decimal Value { get; }

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