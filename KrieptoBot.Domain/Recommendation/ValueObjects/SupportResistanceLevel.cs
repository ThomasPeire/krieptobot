using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using KrieptoBot.Domain.BuildingBlocks;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Recommendation.ValueObjects
{
    public class SupportResistanceLevel : ValueObject
    {
        public SupportResistanceLevel(Price value, DateTime from)
        {
            Value = value;
            From = @from;
        }

        public SupportResistanceLevel(decimal value, DateTime from)
        {
            Value = new Price(value);
            From = @from;
        }

        public Price Value { get; }
        public DateTime From { get; }

        public static implicit operator decimal(SupportResistanceLevel level)
        {
            return level.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return From;
        }
    }
}
