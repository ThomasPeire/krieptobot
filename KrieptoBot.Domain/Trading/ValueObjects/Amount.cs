using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Amount : ValueObject
    {
        private Amount()
        {
        }

        public Amount(decimal value)
        {
            if (value < 0m)
                throw new ArgumentException("Amount can not be negative", nameof(value));

            Value = value;
        }

        public static Amount Zero => new(0);
        public decimal Value { get; }

        public static implicit operator decimal(Amount amount)
        {
            return amount.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static Amount operator *(Amount a, Amount b)
        {
            return new(a.Value * b.Value);
        }

        public static Amount operator /(Amount a, Amount b)
        {
            return new(a.Value / b.Value);
        }

        public static Amount operator +(Amount a, Amount b)
        {
            return new(a.Value + b.Value);
        }

        public static Amount operator -(Amount a, Amount b)
        {
            return new(a.Value - b.Value);
        }
    }
}