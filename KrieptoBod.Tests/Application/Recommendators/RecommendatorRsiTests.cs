using System.Runtime.InteropServices.ComTypes;
using KrieptoBod.Application.Recommendators;
using NUnit.Framework;
using System.Threading.Tasks;
using KrieptoBod.Application;
using Moq;

namespace KrieptoBod.Tests.Application.Recommendators
{
    public class RecommendatorRsiTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
        }

        [Test]
        public async Task RecommendationRsi_ShouldReturn_ScoreOfZeroForNow()
        {
            var recommendator = new RecommendatorRsi(_exchangeServiceMock.Object);

            var result = await recommendator.GetRecommendation("btc-eur");

            Assert.That(result.Score, Is.EqualTo(.0F));
        }

    }
}