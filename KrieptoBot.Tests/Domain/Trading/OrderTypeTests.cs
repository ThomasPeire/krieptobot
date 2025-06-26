using System;
using AwesomeAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading;

public class OrderTypeTests
{
    [Test]
    public void Value_ShouldNot_BeInvalid()
    {
        Action act = () => OrderType.FromString("invalid");

        act.Should().Throw<ArgumentException>().WithMessage("invalid is not a valid order type");
    }

    [Test]
    public void Value_ShouldNot_BeNullOrEmpty()
    {
        Action act = () => OrderType.FromString("");

        act.Should().Throw<ArgumentException>().WithMessage("Value can not be empty");
    }

    [Test]
    public void Value_Should_ImplicitlyConvertString()
    {
        string orderTypeString = OrderType.Limit;

        orderTypeString.Should().Be("limit");
    }

    [Test]
    public void Equality_Should_Work()
    {
        var orderType1 = OrderType.Limit;
        var orderType2 = OrderType.Limit;

        var result = orderType1.Equals(orderType2);

        result.Should().BeTrue();
    }

    [Test]
    public void FromString_Should_ReturnCorrectValueObject()
    {
        var limit = OrderType.FromString("limit");
        var market = OrderType.FromString("market");
        var takeprofitlimit = OrderType.FromString("takeprofitlimit");

        limit.Should().Be(OrderType.Limit);
        market.Should().Be(OrderType.Market);
        takeprofitlimit.Should().Be(OrderType.TakeProfitLimit);
    }
}