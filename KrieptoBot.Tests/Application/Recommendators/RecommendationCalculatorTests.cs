using KrieptoBot.Application.Recommendators;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendationCalculatorTests
    {
        private Mock<IRecommendator> _recommendatorSell150;
        private Mock<IRecommendator> _recommendatorSell70;
        private Mock<IRecommendator> _recommendatorBuy100;

        [SetUp]
        public void Setup()
        {
            _recommendatorSell150 = new Mock<IRecommendator>();
            _recommendatorSell150
                .Setup(x => x.GetRecommendation(It.IsAny<string>()))
                .Returns(Task.FromResult(new RecommendatorScore { Score = -150F }));

            _recommendatorSell70 = new Mock<IRecommendator>();
            _recommendatorSell70
                .Setup(x => x.GetRecommendation(It.IsAny<string>()))
                .Returns(Task.FromResult(new RecommendatorScore { Score = -70F }));

            _recommendatorBuy100 = new Mock<IRecommendator>();
            _recommendatorBuy100
                .Setup(x => x.GetRecommendation(It.IsAny<string>()))
                .Returns(Task.FromResult(new RecommendatorScore { Score = 100F }));

        }

        [Test]
        public async Task RecommendationCalculator_ShouldReturn_SumOfRecommendators()
        {
            var recommendators = new List<IRecommendator>
                {_recommendatorSell150.Object, _recommendatorSell70.Object, _recommendatorBuy100.Object};

            var recommendationCalculator = new RecommendationCalculator(recommendators);

            var result = await recommendationCalculator.CalculateRecommendation("btc-eur");

            Assert.That(result.Score, Is.EqualTo(-120F));
        }

    }
}