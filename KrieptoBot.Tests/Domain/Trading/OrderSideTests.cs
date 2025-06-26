using System;
using AwesomeAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class OrderSideTests
    {
        [Test]
        public void Value_ShouldNot_BeInvalid()
        {
            Func<OrderSide> act = () => OrderSide.FromString("invalid");

            act.Should().Throw<ArgumentException>().WithMessage("invalid is not a valid order side");
        }

        [Test]
        public void Value_ShouldNot_BeNullOrEmpty()
        {
            Func<OrderSide> act = () => OrderSide.FromString("");

            act.Should().Throw<ArgumentException>().WithMessage("Value can not be empty");
        }

        [Test]
        public void Value_Should_ImplicitlyConvertString()
        {
            string orderSideString = OrderSide.Buy;

            orderSideString.Should().Be("buy");
        }


        [Test]
        public void FromString_Should_ReturnCorrectValueObject()
        {
            var buy = OrderSide.FromString("buy");
            var sell = OrderSide.FromString("sell");

            buy.Should().Be(OrderSide.Buy);
            sell.Should().Be(OrderSide.Sell);
        }
    }
}
