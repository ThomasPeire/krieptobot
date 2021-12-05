using System;
using System.Globalization;
using AutoBogus;
using FluentAssertions;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper;
using NUnit.Framework;

namespace KrieptoBot.Tests.Exchange.Bitvavo.Helper
{
    public class HelperMethodsTests
    {
        [TestCase("1m", 1)]
        [TestCase("1m", 1)]
        [TestCase("5m", 5)]
        [TestCase("15m", 15)]
        [TestCase("30m", 30)]
        [TestCase("1h", 60)]
        [TestCase("2h", 120)]
        [TestCase("4h", 240)]
        [TestCase("6h", 360)]
        [TestCase("8h", 480)]
        [TestCase("12h", 720)]
        [TestCase("1d", 1440)]
        [TestCase("", 0)]
        public void GetIntervalInMinutes_Should_ReturnIntervalInMinutes(string intervalString, int expectedMinutes)
        {
            var result = HelperMethods.GetIntervalInMinutes(intervalString);

            result.Should().Be(expectedMinutes);
        }
    }
}
