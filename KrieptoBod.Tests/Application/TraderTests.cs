using KrieptoBod.Application;
using KrieptoBod.Application.Recommendators;
using KrieptoBod.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBod.Tests.Application
{
    public class TraderTests
    {
        private Mock<IRecommendationCalculator> _recommendationCalculator;
        private Mock<IExchangeService> _exchangeServiceMock;

        [SetUp]
        public void Setup()
        {
            _recommendationCalculator = new Mock<IRecommendationCalculator>();
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<string>()))
                .Returns(Task.FromResult(new RecommendatorScore { Score = -150F }));

            _exchangeServiceMock = new Mock<IExchangeService>();
            _exchangeServiceMock
                .Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult<IEnumerable<Candle>>(
                    new List<Candle>
                    {
                        new Candle { Close = 20, High = 40, Low = 10, Open = 30, TimeStamp = new DateTime(2021, 01, 01, 01, 00, 00), Volume = 200 },
                        new Candle { Close = 10, High = 400, Low = 10, Open = 20, TimeStamp = new DateTime(2021, 01, 01, 01, 01, 00), Volume = 200 },
                        new Candle { Close = 1000, High = 1000, Low = 10, Open = 10, TimeStamp = new DateTime(2021, 01, 01, 02, 01, 00), Volume = 200 }
            }));


        }

        [Test]
        public async Task Trader_Should_Trade()
        {
            var trader = new Trader(_exchangeServiceMock.Object, _recommendationCalculator.Object);

            await trader.Run(); 

            Assert.True(true);// todo:proper test
        }

    }
}