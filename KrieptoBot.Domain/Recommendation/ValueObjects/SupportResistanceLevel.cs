using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Recommendation.ValueObjects;

public class SupportResistanceLevel(Price value, DateTime from) : ValueObject
{
    public SupportResistanceLevel(decimal value, DateTime from) : this(new Price(value), from)
    {
    }

    public Price Value { get; } = value;
    public DateTime From { get; } = from;

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