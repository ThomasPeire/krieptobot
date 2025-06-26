using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects;

public class Balance(Symbol symbol, Amount available, Amount inOrder) : ValueObject
{
    public Symbol Symbol { get; } = symbol;
    public Amount Available { get; } = available;
    public Amount InOrder { get; } = inOrder;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Symbol;
        yield return Available;
        yield return InOrder;
    }
}