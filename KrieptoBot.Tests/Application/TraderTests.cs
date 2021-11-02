using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Tests.Application
{
    public class TraderTests
    {
        private Mock<IRecommendationCalculator> _recommendationCalculator;
        private Mock<IExchangeService> _exchangeServiceMock;
        private TradingContext _tradingContext;

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
            _tradingContext = new TradingContext()
                .SetBuyMargin(30)
                .SetSellMargin(-30)
                .SetMarketsToWatch(
                    new List<string>
                    {
                        "BTC-EUR"
                    })
                .SetInterval("5m");

        }

        [Test]
        public async Task Trader_Should_Trade()
        {
            var logger = new Mock<ILogger<Trader>>();
            var trader = new Trader(_exchangeServiceMock.Object, _recommendationCalculator.Object, _tradingContext, logger.Object);

            await trader.Run();

            Assert.True(true);// todo:proper test
        }

    }
}
