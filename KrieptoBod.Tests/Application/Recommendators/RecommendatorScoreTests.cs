using KrieptoBod.Application.Recommendators;
using NUnit.Framework;

namespace KrieptoBod.Tests.Application.Recommendators
{
    public class RecommendatorScoreTests
    {
        
        [Test]
        public void RecommendationScore_ShouldDoMath_Correct()
        {
            const float score1Value = 20;
            const float score2Value = 60;
            var score1 = new RecommendatorScore() { Score = score1Value };
            var score2 = new RecommendatorScore() { Score = score2Value };

            Assert.That((score1 * score2).Score, Is.EqualTo( score1Value * score2Value ));
            Assert.That((score1 / score2).Score, Is.EqualTo( score1Value / score2Value ));
            Assert.That((score1 + score2).Score, Is.EqualTo( score1Value + score2Value ));
            Assert.That((score1 - score2).Score, Is.EqualTo( score1Value - score2Value ));
                        
            Assert.That((score1 * 2).Score, Is.EqualTo(score1Value * 2 ));
            Assert.That((score1 / 2).Score, Is.EqualTo(score1Value / 2 ));
            Assert.That((score1 + 2).Score, Is.EqualTo(score1Value + 2 ));
            Assert.That((score1 - 2).Score, Is.EqualTo(score1Value - 2 ));
        }

    }
}