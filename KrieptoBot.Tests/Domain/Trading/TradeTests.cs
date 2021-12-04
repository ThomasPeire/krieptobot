using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class TradeTests
    {
        [Test]
        public void Timestamp_CanNot_BeInTheFuture()
        {
            Func<Trade> act = () => new Trade(Guid.NewGuid(), DateTime.Today.AddDays(1), new MarketName("eur-btc"),
                Amount.Zero, new Price(10), OrderSide.Buy);

            act.Should().Throw<ArgumentException>().WithMessage("Created datetime can not be in the future");
        }
    }
}
