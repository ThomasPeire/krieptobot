using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application
{
    public class BuyManagerTests
    {
        private Mock<ILogger<BuyManager>> _loggerMock;
        private Mock<ITradingContext> _tradingContextMock;
        private Mock<INotificationManager> _notificationManagerMock;
        private Mock<IExchangeService> _exchangeServiceMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BuyManager>>();
            _tradingContextMock = new Mock<ITradingContext>();
            _notificationManagerMock = new Mock<INotificationManager>();
            _exchangeServiceMock = new Mock<IExchangeService>();
        }

        [Test]
        public async Task BuyManager_ShouldPlaceBuyOrder()
        {
            const int tickerPrice = 3;
            const decimal budget = 50;
            const string market = "btc-eur";

            _tradingContextMock.Setup(x => x.IsSimulation).Returns(false);

            _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>()))
                .ReturnsAsync(new TickerPrice(new MarketName(market), new Price(tickerPrice)));

            var buyManager = new BuyManager(_loggerMock.Object, _tradingContextMock.Object,
                _notificationManagerMock.Object, _exchangeServiceMock.Object);

            await buyManager.Buy(new Market(new MarketName(market), Amount.Zero, Amount.Zero), budget);

            _exchangeServiceMock.Verify(x =>
                x.PostBuyOrderAsync(market, It.IsAny<string>(), budget / tickerPrice, tickerPrice));
            _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }

        [Test]
        public async Task BuyManager_ShouldNotPlaceBuyOrder_WhenInSimulationMode()
        {
            const int tickerPrice = 3;
            const decimal budget = 50;
            const string market = "btc-eur";

            _tradingContextMock.Setup(x => x.IsSimulation).Returns(true);

            _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>()))
                .ReturnsAsync(new TickerPrice(new MarketName(market), new Price(tickerPrice)));

            var buyManager = new BuyManager(_loggerMock.Object, _tradingContextMock.Object,
                _notificationManagerMock.Object, _exchangeServiceMock.Object);

            await buyManager.Buy(new Market(new MarketName(market), Amount.Zero, Amount.Zero), budget);

            _exchangeServiceMock.Verify(x =>
                x.PostBuyOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
            _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }
    }
}
