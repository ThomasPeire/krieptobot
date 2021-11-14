using System;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Trading.Entity
{
    public class Trade : BuildingBlocks.Entity
    {
        public Trade(Guid id, DateTime timestamp, Market market, Amount amount, Price price, OrderSide side) : base(id)
        {
            ArgumentNullException.ThrowIfNull(market);
            ArgumentNullException.ThrowIfNull(timestamp);
            ArgumentNullException.ThrowIfNull(side);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNull(price);

            if (timestamp > DateTime.UtcNow) throw new ArgumentException("Created datetime can not be in the future");

            Timestamp = timestamp;
            Market = market;
            Amount = amount;
            Price = price;
            Side = side;
        }

        public DateTime Timestamp { get; }
        public Market Market { get; }
        public Amount Amount { get; }
        public Price Price { get; }
        public OrderSide Side { get; }
    }
}