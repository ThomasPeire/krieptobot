using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects;

public class Asset(Symbol symbol, AssetName name) : ValueObject
{
    public Symbol Symbol { get; } = symbol;

    public AssetName Name { get; } = name;


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Symbol;
        yield return Name;
    }
}