using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public sealed class OrderSide : ValueObject

    {
        public static readonly OrderSide Sell = new("sell");
        public static readonly OrderSide Buy = new("buy");

        private OrderSide()
        {
        }

        private OrderSide(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator string(OrderSide side)
        {
            return side.Value;
        }

        public static OrderSide FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value can not be empty");

            return value.ToLower() switch
            {
                "buy" => Buy,
                "sell" => Sell,
                _ => throw new ArgumentException($"{value} is not a valid order side")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}