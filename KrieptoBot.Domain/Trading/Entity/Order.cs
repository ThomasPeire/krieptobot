using System;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Trading.Entity
{
    public class Order : BuildingBlocks.Entity
    {
        public Order(Guid id, MarketName marketName, DateTime created, DateTime updated, OrderStatus status,
            OrderSide side,
            OrderType type, Amount amount, Price price) : base(id)
        {
            ArgumentNullException.ThrowIfNull(marketName);
            ArgumentNullException.ThrowIfNull(created);
            ArgumentNullException.ThrowIfNull(updated);
            ArgumentNullException.ThrowIfNull(status);
            ArgumentNullException.ThrowIfNull(side);
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNull(price);

            // var now = DateTime.UtcNow.AddSeconds(10);

            // if (created > now) throw new ArgumentException("Created datetime can not be in the future");
            //
            // if (updated > now) throw new ArgumentException("Updated datetime can not be in the future");
            //
            // if (created > updated)
            //     throw new ArgumentException("Created datetime can not be in the greater than updated datetime");


            MarketName = marketName;
            Created = created;
            Updated = updated;
            Status = status;
            Side = side;
            Type = type;
            Amount = amount;
            Price = price;
        }

        public MarketName MarketName { get; }
        public DateTime Created { get; }
        public DateTime Updated { get; }
        public OrderStatus Status { get; }
        public OrderSide Side { get; }
        public OrderType Type { get; }
        public Amount Amount { get; }
        public Price Price { get; }
    }
}
