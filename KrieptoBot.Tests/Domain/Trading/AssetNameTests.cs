using System;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading
{
    public class AssetNameTests
    {
        [Test]
        public void Value_ShouldNot_BeNullOrEmpty()
        {
            Action act1 = () => new AssetName(null);
            Action act2 = () => new AssetName("");

            act1.Should().Throw<ArgumentException>().WithMessage("Asset name can not be empty");
            act2.Should().Throw<ArgumentException>().WithMessage("Asset name can not be empty");
        }
    }
}
