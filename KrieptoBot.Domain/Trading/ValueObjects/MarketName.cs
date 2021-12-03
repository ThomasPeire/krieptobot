using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class MarketName : ValueObject
    {
        public MarketName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Market name can not be empty", nameof(name));

            var splittedName = name.Split("-");

            if (splittedName.Length != 2)
                throw new ArgumentException("Market name is not correctly formatted", nameof(name));

            BaseSymbol = new Symbol(splittedName[0]);
            QuoteSymbol = new Symbol(splittedName[1]);
        }

        public MarketName(Symbol baseSymbol, Symbol quoteSymbol)
        {
            ArgumentNullException.ThrowIfNull(baseSymbol);
            ArgumentNullException.ThrowIfNull(quoteSymbol);
            BaseSymbol = baseSymbol;
            QuoteSymbol = quoteSymbol;
        }

        public MarketName(string baseSymbol, string quoteSymbol) : this(new Symbol(baseSymbol), new Symbol(quoteSymbol))
        {
        }

        public string Value => $"{BaseSymbol.Value}-{QuoteSymbol.Value}";
        public Symbol BaseSymbol { get; }
        public Symbol QuoteSymbol { get; }

        public static implicit operator string(MarketName marketName)
        {
            return marketName.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
