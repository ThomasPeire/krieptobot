using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application
{
    public class TradingContextTests
    {
        [Test]
        public void TradingContext_Should_SetProperties()
        {
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.Setup(x => x.UtcDateTimeNow()).Returns(Task.FromResult(new DateTime(2001, 1, 1)));

            var tradingContext = new TradingContext(mockDateTimeProvider.Object);

            tradingContext.SetCurrentTime();
            tradingContext.SetInterval("4h");
            tradingContext.SetIsSimulation(false);
            tradingContext.SetBuyMargin(1);
            tradingContext.SetSellMargin(2);
            tradingContext.SetMarketsToWatch(new List<string> { "market" });

            Assert.That(tradingContext.CurrentTime, Is.EqualTo(new DateTime(2001, 1, 1)));
            Assert.That(tradingContext.Interval, Is.EqualTo("4h"));
            Assert.That(tradingContext.IsSimulation, Is.EqualTo(false));
            Assert.That(tradingContext.BuyMargin, Is.EqualTo(1));
            Assert.That(tradingContext.SellMargin, Is.EqualTo(2));
            Assert.That(tradingContext.MarketsToWatch.First(), Is.EqualTo("market"));
        }
    }
}
