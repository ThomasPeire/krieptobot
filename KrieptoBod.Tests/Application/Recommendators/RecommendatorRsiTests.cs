using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KrieptoBod.Application.Recommendators;
using NUnit.Framework;
using System.Threading.Tasks;
using KrieptoBod.Application;
using KrieptoBod.Application.Indicators;
using KrieptoBod.Model;
using Moq;

namespace KrieptoBod.Tests.Application.Recommendators
{
    public class RecommendatorRsiTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<IRsi> _rsiIndicator;

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
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_PositiveScoreWhenRSIAbove70()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 80},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());
            
            Assert.That(result.Score, Is.EqualTo((80F - 70) / 30));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_NegativeScoreWhenRSIUnder30()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 15},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());

            Assert.That(result.Score, Is.EqualTo((15F - 30) / 30));
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_ZeroScoreWhenRSIBetween30And70()
        {
            var rsiResults =
                new Dictionary<DateTime, decimal>
                {
                    {DateTime.Today, 49},
                };

            _rsiIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>(), It.IsAny<int>()))
                .Returns(rsiResults);

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object);

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

            var recommendator = new RecommendatorRsi14(_exchangeServiceMock.Object, _rsiIndicator.Object);

            var result = await recommendator.GetRecommendation(It.IsAny<string>());

            Assert.That(result.Score, Is.EqualTo(((float)todaysRsiValue - 30) / 30));
        }

    }
}