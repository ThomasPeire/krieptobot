using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorSupportTests
    {
        [Test]
        public async Task RecommendationSupport_ShouldReturn_ScoreOfZeroForNow()
        {
            var recommendatorSettingOptions =
                new Mock<IOptions<RecommendatorSettings>>();

            recommendatorSettingOptions.Setup(x => x.Value).Returns(new RecommendatorSettings
            {
                BuyRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorSupport), 0m }
                },
                SellRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorSupport), 0m }
                }
            });

            var recommendator = new RecommendatorSupport(recommendatorSettingOptions.Object);

            var result = await recommendator.GetRecommendation(new Market("btc-eur"));

            Assert.That(result.Value, Is.EqualTo(.0m));
        }
    }
}
