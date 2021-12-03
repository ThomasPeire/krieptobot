using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class MarketNameTests
    {
        [Test]
        public void Name_ShouldNot_BeNullOrEmpty()
        {
            Action act1 = () =>
                new MarketName(null);
            Action act2 = () =>
                new MarketName("");

            act1.Should().Throw<ArgumentException>().WithMessage("Market name can not be empty (Parameter 'name')");
            act2.Should().Throw<ArgumentException>().WithMessage("Market name can not be empty (Parameter 'name')");
        }

        [Test]
        public void Name_Should_HaveCorrectFormat()
        {
            Action act = () =>
                new MarketName("incorrect format");

            act.Should().Throw<ArgumentException>().WithMessage("Market name is not correctly formatted (Parameter 'name')");
        }
    }
}
