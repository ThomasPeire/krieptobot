using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class SymbolTests
    {
        [Test]
        public void Value_ShouldNot_BeNullOrEmpty()
        {
            Action act1 = () => new Symbol(null);
            Action act2 = () => new Symbol("");

            act1.Should().Throw<ArgumentException>().WithMessage("Symbol can not be empty");
            act2.Should().Throw<ArgumentException>().WithMessage("Symbol can not be empty");
        }
    }
}
