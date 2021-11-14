using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorRsiTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<ILogger<RecommendatorRsi14PeriodInterval>> _logger;
        private Mock<IRsi> _rsiIndicator;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _rsiIndicator = new Mock<IRsi>();
            _logger = new Mock<ILogger<RecommendatorRsi14PeriodInterval>>();

            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 80 },
                    { DateTime.Today.AddDays(-1), 70 },
                    { DateTime.Today.AddDays(-2), 60 },
                    { DateTime.Today.AddDays(-3), 50 },
                    { DateTime.Today.AddDays(-4), 40 }
                };

            _tradingContext = new TradingContext()
                .SetBuyMargin(30)
                .SetSellMargin(-30)
                .SetMarketsToWatch(
                    new List<string>
                    {
                        "BTC-EUR"
                    })
                .SetInterval("5m");
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_NegativeScoreWhenRSIAbove50()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 80 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14PeriodInterval(_exchangeServiceMock.Object, _rsiIndicator.Object,
                _tradingContext, _logger.Object);

            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.LessThan(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_PositiveScoreWhenRSIUnder50()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 15 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14PeriodInterval(_exchangeServiceMock.Object, _rsiIndicator.Object,
                _tradingContext, _logger.Object);

            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.GreaterThan(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_HigherRecommendationForLowerRSI()
        {
            var rsiResults1 =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 15 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults1);

            var recommendator = new RecommendatorRsi14PeriodInterval(_exchangeServiceMock.Object, _rsiIndicator.Object,
                _tradingContext, _logger.Object);

            var result1 = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            var rsiResults2 =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 80 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults2);

            var result2 = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result1.Value, Is.GreaterThan(result2.Value));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_ZeroScoreWhenRSIIs50()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 50 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator =
                new RecommendatorRsi14PeriodInterval(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext,
                    _logger.Object);

            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.EqualTo(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldWorkWith_LastRsiValueFromRsiIndicator()
        {
            var todaysRsiValue = 16;
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today.AddDays(-2), 98 },
                    { DateTime.Today.AddDays(-5), 32 },
                    { DateTime.Today.AddDays(-3), 13 },
                    { DateTime.Today, todaysRsiValue },
                    { DateTime.Today.AddDays(-1), 98 },
                    { DateTime.Today.AddDays(-4), 84 }
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14PeriodInterval(_exchangeServiceMock.Object, _rsiIndicator.Object,
                _tradingContext, _logger.Object);

            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.EqualTo(100 - todaysRsiValue * 2));
        }
    }
}