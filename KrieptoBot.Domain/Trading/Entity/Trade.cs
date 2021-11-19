using System;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Domain.Trading.Entity
{
    public class Trade : BuildingBlocks.Entity
    {
        public Trade(Guid id, DateTime timestamp, MarketName marketName, Amount amount, Price price,
            OrderSide side) : base(id)
        {
            ArgumentNullException.ThrowIfNull(marketName);
            ArgumentNullException.ThrowIfNull(timestamp);
            ArgumentNullException.ThrowIfNull(side);
            ArgumentNullException.ThrowIfNull(amount);
            ArgumentNullException.ThrowIfNull(price);

            if (timestamp > DateTime.UtcNow) throw new ArgumentException("Created datetime can not be in the future");

            Timestamp = timestamp;
            MarketName = marketName;
            Amount = amount;
            Price = price;
            Side = side;
        }

        public DateTime Timestamp { get; }
        public MarketName MarketName { get; }
        public Amount Amount { get; }
        public Price Price { get; }
        public OrderSide Side { get; }
    }
}