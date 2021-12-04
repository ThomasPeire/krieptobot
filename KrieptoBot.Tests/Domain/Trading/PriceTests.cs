using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class PriceTests
    {
        [Test]
        public void Value_ShouldNot_BeNegative()
        {
            Func<Price> act = () => new Price(-10);

            act.Should().Throw<ArgumentException>().WithMessage("Price can not be negative (Parameter 'value')");
        }
    }
}
