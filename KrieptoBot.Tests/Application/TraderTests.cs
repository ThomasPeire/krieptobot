using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application
{
    public class TraderTests
    {
        private Mock<IBuyManager> _buyManagerMock;
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<IRecommendationCalculator> _recommendationCalculator;
        private Mock<ISellManager> _sellManagerMock;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _buyManagerMock = new Mock<IBuyManager>();
            _sellManagerMock = new Mock<ISellManager>();
            _recommendationCalculator = new Mock<IRecommendationCalculator>();
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(-99)));

            _exchangeServiceMock = new Mock<IExchangeService>();
            _exchangeServiceMock
                .Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult<IEnumerable<Candle>>(
                    new List<Candle>
                    {
                        new(new DateTime(2021, 01, 01, 01, 00, 00), new Price(40), new Price(10), new Price(30),
                            new Price(20), 200)
                    }));
            _exchangeServiceMock
                .Setup(x => x.GetMarketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero)));
            _tradingContext = new TradingContext(new DateTimeProvider())
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
            var trader = new Trader(logger.Object, _tradingContext, _exchangeServiceMock.Object,
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object);

            await trader.Run();

            Assert.True(true); // todo:proper test
        }
    }
}
