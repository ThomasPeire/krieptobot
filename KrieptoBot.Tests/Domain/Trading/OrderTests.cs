using System;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class OrderTests
    {
        // [Test]
        // public void Created_CanNot_BeInTheFuture()
        // {
        //     var act = () => new Order(Guid.NewGuid(), new MarketName("eur-btc"), DateTime.Today.AddDays(1),
        //         DateTime.Today, OrderStatus.New, OrderSide.Buy, OrderType.Limit, Amount.Zero, new Price(10), new Price(0));
        //
        //     act.Should().Throw<ArgumentException>().WithMessage("Created datetime can not be in the future");
        // }
        //
        // [Test]
        // public void Updated_CanNot_BeInTheFuture()
        // {
        //     Func<Order> act = () => new Order(Guid.NewGuid(), new MarketName("eur-btc"), DateTime.Today,
        //         DateTime.Today.AddDays(1), OrderStatus.New, OrderSide.Buy, OrderType.Limit, Amount.Zero, new Price(10), new Price(0));
        //
        //     act.Should().Throw<ArgumentException>().WithMessage("Updated datetime can not be in the future");
        // }
        //
        // [Test]
        // public void Created_CanNot_BeAfterUpdated()
        // {
        //     Func<Order> act = () => new Order(Guid.NewGuid(), new MarketName("eur-btc"), DateTime.Today.AddDays(-1),
        //         DateTime.Today.AddDays(-2), OrderStatus.New, OrderSide.Buy, OrderType.Limit, Amount.Zero, new Price(10), new Price(0));
        //
        //     act.Should().Throw<ArgumentException>().WithMessage("Created datetime can not be greater than updated datetime");
        // }
    }
}
