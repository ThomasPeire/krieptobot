﻿using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Recommendation.ValueObjects
{
    public class RecommendatorScore : ValueObject
    {
        private RecommendatorScore()
        {
        }

        public RecommendatorScore(decimal value, bool includeInAverageScore = true)
        {
            if (value is < 0m or > 100m)
                throw new ArgumentOutOfRangeException(nameof(value), "Score must be positive and not greater than 100");

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
        }

        public static RecommendatorScore operator *(RecommendatorScore recommendatorScore, decimal b)
        {
            return new(recommendatorScore + b, recommendatorScore.IncludeInAverageScore);
        }
    }
}