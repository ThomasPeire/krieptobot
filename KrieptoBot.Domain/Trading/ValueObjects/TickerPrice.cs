using System;
using System.Collections.Generic;
using KrieptoBot.Domain.BuildingBlocks;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class TickerPrice : ValueObject
    {
        private TickerPrice()
        {
        }

        public TickerPrice(MarketName marketName, Price price)
        {
            ArgumentNullException.ThrowIfNull(marketName);
            ArgumentNullException.ThrowIfNull(price);

            MarketName = marketName;
            Price = price;
        }

        public MarketName MarketName { get; }
        public Price Price { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MarketName;
            yield return Price;
        }
    }
}