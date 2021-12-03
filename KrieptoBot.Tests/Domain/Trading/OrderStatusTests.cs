using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class OrderStatusTests
    {
        [Test]
        public void Value_ShouldNot_BeInvalid()
        {
            Action act = () => OrderStatus.FromString("invalid");

            act.Should().Throw<ArgumentException>().WithMessage("invalid is not a valid order status");
        }

        [Test]
        public void Value_ShouldNot_BeNullOrEmpty()
        {
            Action act = () => OrderStatus.FromString("");

            act.Should().Throw<ArgumentException>().WithMessage("Value can not be empty");
        }

        [Test]
        public void Value_Should_ImplicitlyConvertString()
        {
            string orderStatusString = OrderStatus.Filled;

            orderStatusString.Should().Be("filled");
        }

        [Test]
        public void Equality_Should_Work()
        {
            var status1 = OrderStatus.Filled;
            var status2 = OrderStatus.Filled;

            var result = status1.Equals(status2);

            result.Should().BeTrue();
        }

        [Test]
        public void FromString_Should_ReturnCorrectValueObject()
        {
            var canceled = OrderStatus.FromString("canceled");
            var expired = OrderStatus.FromString("expired");
            var filled = OrderStatus.FromString("filled");
            var @new = OrderStatus.FromString("new");
            var rejected = OrderStatus.FromString("rejected");
            var awaitingtrigger = OrderStatus.FromString("awaitingtrigger");
            var canceledauction = OrderStatus.FromString("canceledauction");
            var canceledioc = OrderStatus.FromString("canceledioc");
            var canceledfok = OrderStatus.FromString("canceledfok");
            var partiallyfilled = OrderStatus.FromString("partiallyfilled");
            var canceledmarketprotection = OrderStatus.FromString("canceledmarketprotection");
            var canceledpostonly = OrderStatus.FromString("canceledpostonly");
            var canceledselftradeprevention = OrderStatus.FromString("canceledselftradeprevention");

            canceled.Should().Be(OrderStatus.Canceled);
            expired.Should().Be(OrderStatus.Expired);
            filled.Should().Be(OrderStatus.Filled);
            @new.Should().Be(OrderStatus.New);
            rejected.Should().Be(OrderStatus.Rejected);
            awaitingtrigger.Should().Be(OrderStatus.AwaitingTrigger);
            canceledauction.Should().Be(OrderStatus.CanceledAuction);
            canceledioc.Should().Be(OrderStatus.CanceledIoc);
            canceledfok.Should().Be(OrderStatus.CanceledFok);
            partiallyfilled.Should().Be(OrderStatus.PartiallyFilled);
            canceledmarketprotection.Should().Be(OrderStatus.CanceledMarketProtection);
            canceledpostonly.Should().Be(OrderStatus.CanceledPostOnly);
            canceledselftradeprevention.Should().Be(OrderStatus.CanceledSelfTradePrevention);
        }
    }
}
