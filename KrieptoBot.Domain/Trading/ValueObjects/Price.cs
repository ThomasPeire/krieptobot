using System;
using System.Collections.Generic;
using System.Globalization;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Price : ValueObject
    {
        private Price()
        {
        }

        public Price(decimal value)
        {
            if (value < 0m)
                throw new ArgumentException("Price can not be negative", nameof(value));

            Value = value;
        }

        public decimal Value { get; }

        public static implicit operator decimal(Price price)
        {
            return price.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }


    }
}
