using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public sealed class OrderType : ValueObject

    {
        public static readonly OrderType Limit = new("limit");
        public static readonly OrderType TakeProfitLimit = new("takeProfitLimit");
        public static readonly OrderType Market = new("market");

        private OrderType()
        {
        }

        private OrderType(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator string(OrderType orderType)
        {
            return orderType.Value;
        }

        public static OrderType FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value can not be empty");

            return value.ToLower() switch
            {
                "limit" => Limit,
                "takeprofitlimit" => TakeProfitLimit,
                "market" => Market,
                _ => throw new ArgumentException($"{value} is not a valid ordertype")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
