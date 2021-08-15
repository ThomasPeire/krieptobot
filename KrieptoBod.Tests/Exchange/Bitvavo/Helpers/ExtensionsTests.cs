using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBod.Application;
using KrieptoBod.Application.Recommendators;
using KrieptoBod.Model;
using Moq;
using NUnit.Framework;

namespace KrieptoBod.Tests.Exchange.Bitvavo.Helpers
{
    public class ExtensionsTests
    {
        private Mock<IRecommendationCalculator> _recommendationCalculator;
        private Mock<IRepository> _repository;

        [SetUp]
        public void Setup()
        {
            _recommendationCalculator = new Mock<IRecommendationCalculator>();
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<string>()))
                .Returns(Task.FromResult(new RecommendatorScore() { Score = -150F }));

            _repository = new Mock<IRepository>();
            _repository
                .Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult<IEnumerable<Candle>>(
                    new List<Candle>()
                    {
                        new Candle() { Close = 20, High = 40, Low = 10, Open = 30, TimeStamp = new DateTime(01, 01, 2021, 01, 00, 00), Volume = 200 },
                        new Candle() { Close = 10, High = 400, Low = 10, Open = 20, TimeStamp = new DateTime(01, 01, 2021, 01, 01, 00), Volume = 200 },
                        new Candle() { Close = 1000, High = 1000, Low = 10, Open = 10, TimeStamp = new DateTime(01, 01, 2021, 02, 01, 00), Volume = 200 }
            }));


        }

        [Test]
        public async Task ConvertToKrieptoBodModel_ShouldConvert_BitvavoAssetToKrieptoBodAsset()
        {
            var trader = new Trader(_repository.Object, _recommendationCalculator.Object);

            await trader.Run(); 

            Assert.That(result.Score, Is.EqualTo(-120F));
        }

    }
}