using KrieptoBot.Application.Recommendators;
using NUnit.Framework;
using System.Threading.Tasks;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorSupportTests
    {
        [Test]
        public async Task RecommendationSupport_ShouldReturn_ScoreOfZeroForNow()
        {
            var recommendator = new RecommendatorSupport();

            var result = await recommendator.GetRecommendation("btc-eur");

            Assert.That(result.Score, Is.EqualTo(.0F));
        }

    }
}