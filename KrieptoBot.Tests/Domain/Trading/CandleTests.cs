using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class CandleTests
    {
        [Test]
        public void Volume_ShouldNot_BeNegative()
        {
            Func<Candle> act = () =>
                new Candle(DateTime.Now, new Price(100), new Price(100), new Price(100), new Price(100), -100);

            act.Should().Throw<ArgumentException>().WithMessage("Volume can not be negative (Parameter 'volume')");
        }
    }
}
