using KrieptoBot.Domain.Recommendation.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorScoreTests
    {
        [Test]
        public void RecommendationScore_ShouldDoMath_Correct()
        {
            const decimal score1Value = 20;
            var score1 = new RecommendatorScore(score1Value);

            Assert.That(score1 * 2, Is.EqualTo(score1Value * 2));
            Assert.That(score1 / 2, Is.EqualTo(score1Value / 2));
            Assert.That(score1 + 2, Is.EqualTo(score1Value + 2));
            Assert.That(score1 - 2, Is.EqualTo(score1Value - 2));
        }
    }
}
