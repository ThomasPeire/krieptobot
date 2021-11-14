using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendationCalculatorTests
    {
        private Mock<ILogger<RecommendationCalculator>> _logger;
        private Mock<IRecommendator> _recommendatorBuy90;
        private Mock<IRecommendator> _recommendatorSell50;
        private Mock<IRecommendator> _recommendatorSell70;

        [SetUp]
        public void Setup()
        {
            _recommendatorSell50 = new Mock<IRecommendator>();
            _recommendatorSell50
                .Setup(x => x.GetRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(-50)));

            _recommendatorSell70 = new Mock<IRecommendator>();
            _recommendatorSell70
                .Setup(x => x.GetRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(-70)));

            _recommendatorBuy90 = new Mock<IRecommendator>();
            _recommendatorBuy90
                .Setup(x => x.GetRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(90)));


            _logger = new Mock<ILogger<RecommendationCalculator>>();
        }

        [Test]
        public async Task RecommendationCalculator_ShouldReturn_AvgOfRecommendators()
        {
            var recommendators = new List<IRecommendator>
                { _recommendatorSell50.Object, _recommendatorSell70.Object, _recommendatorBuy90.Object };

            var recommendationCalculator = new RecommendationCalculator(_logger.Object, recommendators);

            var result =
                await recommendationCalculator.CalculateRecommendation(new Market("btc-eur"));

            Assert.That(result.Value, Is.EqualTo(-10));
        }
    }
}
