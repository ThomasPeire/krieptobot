using AwesomeAssertions;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using NUnit.Framework;

namespace KrieptoBot.Tests.Domain.Recommendation;

public class RecommendationScoreTests
{
    [Test]
    public void ScoresWithSameProperties_ShouldBe_Equal()
    {
        var score1 = new RecommendatorScore(100, false);
        var score2 = new RecommendatorScore(100, false);

        var result = score1.Equals(score2);

        result.Should().BeTrue();
    }
}