using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects;

public class Asset : ValueObject
{
    public Asset(Symbol symbol, AssetName name)
    {
        Symbol = symbol;
        Name = name;
    }

    public Symbol Symbol { get; }

    public AssetName Name { get; }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Symbol;
        yield return Name;
    }
}