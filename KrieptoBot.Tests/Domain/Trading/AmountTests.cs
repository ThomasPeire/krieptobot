using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class AmountTests
    {
        [Test]
        public void Value_ShouldNot_BeNegative()
        {
            Action act = () => new Amount(-10);

            act.Should().Throw<ArgumentException>().WithMessage("Amount can not be negative (Parameter 'value')");
        }

        [Test]
        public void Amount_Can_DoMath()
        {
            var amount10 = new Amount(10);
            var amount5 = new Amount(5);

            (amount10 * amount5).Should().Be(new Amount(50));
            (amount10 / amount5).Should().Be(new Amount(2));
            (amount10 + amount5).Should().Be(new Amount(15));
            (amount10 - amount5).Should().Be(new Amount(5));
        }


    }
}
