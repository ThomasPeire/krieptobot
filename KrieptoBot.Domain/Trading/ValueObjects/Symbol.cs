using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Symbol : ValueObject
    {
        private Symbol()
        {
        }

        public Symbol(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Symbol can not be empty");
            Value = value;
        }

        public string Value { get; }

        public static implicit operator string(Symbol symbol)
        {
            return symbol.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}