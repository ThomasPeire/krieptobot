using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public sealed class OrderStatus : ValueObject
    {
        public static readonly OrderStatus New = new("new");
        public static readonly OrderStatus AwaitingTrigger = new("awaitingTrigger");
        public static readonly OrderStatus Canceled = new("canceled");
        public static readonly OrderStatus CanceledAuction = new("canceledAuction");
        public static readonly OrderStatus CanceledSelfTradePrevention = new("canceledSelfTradePrevention");
        public static readonly OrderStatus CanceledIoc = new("canceledIOC");
        public static readonly OrderStatus CanceledFok = new("canceledFOK");
        public static readonly OrderStatus CanceledMarketProtection = new("canceledMarketProtection");
        public static readonly OrderStatus CanceledPostOnly = new("canceledPostOnly");
        public static readonly OrderStatus Filled = new("filled");
        public static readonly OrderStatus PartiallyFilled = new("partiallyFilled");
        public static readonly OrderStatus Expired = new("expired");
        public static readonly OrderStatus Rejected = new("rejected");

        private OrderStatus()
        {
        }

        private OrderStatus(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator string(OrderStatus status)
        {
            return status.Value;
        }

        public static OrderStatus FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value can not be empty");

            return value.ToLower() switch
            {
                "canceled" => Canceled,
                "expired" => Expired,
                "filled" => Filled,
                "new" => New,
                "rejected" => Rejected,
                "awaitingtrigger" => AwaitingTrigger,
                "canceledauction" => CanceledAuction,
                "canceledioc" => CanceledIoc,
                "canceledfok" => CanceledFok,
                "partiallyfilled" => PartiallyFilled,
                "canceledmarketprotection" => CanceledMarketProtection,
                "canceledpostonly" => CanceledPostOnly,
                "canceledselftradeprevention" => CanceledSelfTradePrevention,
                _ => throw new ArgumentException($"{value} is not a valid order status")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}