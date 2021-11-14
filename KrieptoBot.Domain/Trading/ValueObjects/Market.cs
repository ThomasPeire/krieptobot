using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Market : ValueObject
    {
        private Market()
        {
        }

        public Market(Symbol baseSymbol, Symbol quoteSymbol)
        {
            ArgumentNullException.ThrowIfNull(baseSymbol);
            ArgumentNullException.ThrowIfNull(quoteSymbol);
            BaseSymbol = baseSymbol;
            QuoteSymbol = quoteSymbol;
        }

        public Market(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Market name can not be empty");

            var splittedName = name.Split("-");

            if (splittedName.Length != 2) throw new ArgumentException("Market name is not correctly formatted");

            BaseSymbol = new Symbol(splittedName[0]);
            QuoteSymbol = new Symbol(splittedName[1]);
        }

        public string Name => $"{BaseSymbol.Value}-{QuoteSymbol.Value}";
        public Symbol BaseSymbol { get; }

        public Symbol QuoteSymbol { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BaseSymbol;
            yield return QuoteSymbol;
        }
    }
}
