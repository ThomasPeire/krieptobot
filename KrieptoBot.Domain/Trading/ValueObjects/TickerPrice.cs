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

        public TickerPrice(Market market, Price price)
        {
            ArgumentNullException.ThrowIfNull(market);
            ArgumentNullException.ThrowIfNull(price);

            Market = market;
            Price = price;
        }

        public Market Market { get; }
        public Price Price { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Market;
            yield return Price;
        }
    }
}