using System;
using AwesomeAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Trading;

public class AssetNameTests
{
    [Test]
    public void Value_ShouldNot_BeNullOrEmpty()
    {
        Func<AssetName> act1 = () => new AssetName(null);
        Func<AssetName> act2 = () => new AssetName("");

        act1.Should().Throw<ArgumentException>().WithMessage("Asset name can not be empty");
        act2.Should().Throw<ArgumentException>().WithMessage("Asset name can not be empty");
    }
}