using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KrieptoBot.Application.Recommendators;
using NUnit.Framework;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Model;
using Moq;
using NUnit.Framework.Internal.Commands;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorRsiTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<IRsi> _rsiIndicator;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _rsiIndicator = new Mock<IRsi>();
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 80},
                    {DateTime.Today.AddDays(-1), 70},
                    {DateTime.Today.AddDays(-2), 60},
                    {DateTime.Today.AddDays(-3), 50},
                    {DateTime.Today.AddDays(-4), 40},
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
                    {DateTime.Today, 80},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());
            
            Assert.That(result.Score, Is.LessThan(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_PositiveScoreWhenRSIUnder50()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 15},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());

            Assert.That(result.Score, Is.GreaterThan(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_HigherRecommendationForLowerRSI()
        {
            var rsiResults1 =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 15},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults1);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext);

            var result1 = await recommendator.GetRecommendation(It.IsAny<string>());


            var rsiResults2 =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 80},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults2);

            var result2 = await recommendator.GetRecommendation(It.IsAny<string>());


            Assert.That(result1.Score, Is.GreaterThan(result2.Score));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_ZeroScoreWhenRSIIs50()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 50},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());

            Assert.That(result.Score, Is.EqualTo(0));
        }

        [Test]
        public async Task RecommendationRsi_ShouldWorkWith_LastRsiValueFromRsiIndicator()
        {
            var todaysRsiValue = 16;
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today.AddDays(-2), 98},
                    {DateTime.Today.AddDays(-5), 32},
                    {DateTime.Today.AddDays(-3), 13},
                    {DateTime.Today, todaysRsiValue},
                    {DateTime.Today.AddDays(-1), 98},
                    {DateTime.Today.AddDays(-4), 84},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object, _tradingContext);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());

            Assert.That(result.Score, Is.EqualTo(((float)50 - todaysRsiValue) / 100 *2));
        }

    }
}