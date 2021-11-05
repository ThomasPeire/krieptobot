using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Model;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorSupportTests
    {
        [Test]
        public async Task RecommendationSupport_ShouldReturn_ScoreOfZeroForNow()
        {
            var recommendator = new RecommendatorSupport();

            var result = await recommendator.GetRecommendation(new Market() { MarketName = "btc-eur" });

            Assert.That(result.Score, Is.EqualTo(.0m));
        }
    }
}
