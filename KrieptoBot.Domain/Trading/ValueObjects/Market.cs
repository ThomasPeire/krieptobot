using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects;

public class Market : ValueObject
{
    public Market(MarketName name, Amount minimumBaseAmount, Amount minimumQuoteAmount)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(minimumBaseAmount);
        ArgumentNullException.ThrowIfNull(minimumQuoteAmount);

        MinimumBaseAmount = minimumBaseAmount;
        MinimumQuoteAmount = minimumQuoteAmount;
        Name = name;
    }

    public MarketName Name { get; }
    public Amount MinimumBaseAmount { get; }
    public Amount MinimumQuoteAmount { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return MinimumBaseAmount;
        yield return MinimumQuoteAmount;
    }
}