using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application;

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
        const decimal budget = 50;
        const string market = "btc-eur";
        const int currentHigh = 100;
        const int currentLow = 10;

        _tradingContextMock.Setup(x => x.IsSimulation).Returns(false);

        _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>()))
            .ReturnsAsync(new TickerPrice(new MarketName(market), new Price(10000)));

        _exchangeServiceMock.Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Candle>(new[]
            {
                new Candle(DateTime.Today, new Price(currentHigh), new Price(currentLow), new Price(currentHigh),
                    new Price(currentLow), 100),
                new Candle(DateTime.Today.AddDays(-1), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-2), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-3), new Price(3), new Price(3), new Price(3), new Price(3), 100)
            }));
        var buyManager = new BuyManager(_loggerMock.Object, _tradingContextMock.Object,
            _notificationManagerMock.Object, _exchangeServiceMock.Object);

        await buyManager.Buy(new Market(new MarketName(market), Amount.Zero, Amount.Zero), budget);
        const int priceToBuy = currentLow + (currentHigh - currentLow) / 2;

        _exchangeServiceMock.Verify(x =>
            x.PostBuyOrderAsync(market, It.IsAny<string>(), budget / priceToBuy, priceToBuy));
        _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    // var lastCandles = await _exchangeService.GetCandlesAsync(market.Name, _tradingContext.Interval,
    //     end: _tradingContext.CurrentTime);
    // var lastCandle = lastCandles.OrderByDescending(x => x.TimeStamp).First();
    // var high = Math.Max(lastCandle.Close, lastCandle.Open);
    // var low = Math.Min(lastCandle.Close, lastCandle.Open);
    //
    //     return new TickerPrice(market.Name, new Price(low + (high - low) / 2));

    [Test]
    public async Task BuyManager_ShouldNotPlaceBuyOrder_WhenInSimulationMode()
    {
        const decimal budget = 50;
        const string market = "btc-eur";
        const int currentHigh = 100;
        const int currentLow = 10;

        _tradingContextMock.Setup(x => x.IsSimulation).Returns(true);

        _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>()))
            .ReturnsAsync(new TickerPrice(new MarketName(market), new Price(10000)));
        _exchangeServiceMock.Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Candle>(new[]
            {
                new Candle(DateTime.Today, new Price(currentHigh), new Price(currentLow), new Price(3), new Price(3),
                    100),
                new Candle(DateTime.Today.AddDays(-1), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-2), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-3), new Price(3), new Price(3), new Price(3), new Price(3), 100)
            }));

        var buyManager = new BuyManager(_loggerMock.Object, _tradingContextMock.Object,
            _notificationManagerMock.Object, _exchangeServiceMock.Object);

        await buyManager.Buy(new Market(new MarketName(market), Amount.Zero, Amount.Zero), budget);

        _exchangeServiceMock.Verify(x =>
                x.PostBuyOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()),
            Times.Never);
        _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}