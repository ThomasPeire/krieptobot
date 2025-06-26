using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application;

public class SellManagerTests
{
    private Mock<ILogger<SellManager>> _loggerMock;
    private Mock<ITradingContext> _tradingContextMock;
    private Mock<INotificationManager> _notificationManagerMock;
    private Mock<IExchangeService> _exchangeServiceMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<SellManager>>();
        _tradingContextMock = new Mock<ITradingContext>();
        _notificationManagerMock = new Mock<INotificationManager>();
        _exchangeServiceMock = new Mock<IExchangeService>();
    }

    [Test]
    public async Task SellManager_ShouldPlaceSellOrder()
    {
        const decimal amountInOrder = 20;
        const decimal amountAvailable = 100;
        const string market = "btc-eur";
        const int currentHigh = 100;
        const int currentLow = 10;
        const decimal tickerPrice = 4;

        _tradingContextMock.Setup(x => x.IsSimulation).Returns(false);

        _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(new TickerPrice(new MarketName(market), new Price(tickerPrice)));
        _exchangeServiceMock.Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Candle>(new[]
            {
                new Candle(DateTime.Today, new Price(currentHigh), new Price(currentLow), new Price(currentHigh), new Price(currentLow), 100),
                new Candle(DateTime.Today.AddDays(-1), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-2), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-3), new Price(3), new Price(3), new Price(3), new Price(3), 100)
            }));
        _exchangeServiceMock.Setup(x => x.GetBalanceAsync(new Symbol("btc")))
            .ReturnsAsync(new Balance(new Symbol("btc"), new Amount(amountAvailable), new Amount(amountInOrder)));

        var sellManager = new SellManager(_loggerMock.Object, _tradingContextMock.Object,
            _notificationManagerMock.Object, _exchangeServiceMock.Object);

        await sellManager.Sell(new Market(new MarketName(market), Amount.Zero, Amount.Zero));

        const decimal priceToSell = tickerPrice;

        _exchangeServiceMock.Verify(x =>
            x.PostSellOrderAsync(market, It.IsAny<string>(), amountAvailable + amountInOrder, priceToSell));
        _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task SellManager_ShouldNotPlaceSellOrder_WhenInSimulationMode()
    {
        const decimal amountInOrder = 20;
        const decimal amountAvailable = 100;
        const string market = "btc-eur";
        const int currentHigh = 100;
        const int currentLow = 10;

        _tradingContextMock.Setup(x => x.IsSimulation).Returns(true);

        _exchangeServiceMock.Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(new TickerPrice(new MarketName(market), new Price(10000)));
        _exchangeServiceMock.Setup(x => x.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Candle>(new[]
            {
                new Candle(DateTime.Today, new Price(currentHigh), new Price(currentLow), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-1), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-2), new Price(3), new Price(3), new Price(3), new Price(3), 100),
                new Candle(DateTime.Today.AddDays(-3), new Price(3), new Price(3), new Price(3), new Price(3), 100)
            }));

        _exchangeServiceMock.Setup(x => x.GetBalanceAsync("btc"))
            .ReturnsAsync(new Balance(new Symbol("btc"), new Amount(amountAvailable), new Amount(amountInOrder)));

        var sellManager = new SellManager(_loggerMock.Object, _tradingContextMock.Object,
            _notificationManagerMock.Object, _exchangeServiceMock.Object);

        await sellManager.Sell(new Market(new MarketName(market), Amount.Zero, Amount.Zero));

        _exchangeServiceMock.Verify(x =>
                x.PostSellOrderAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                    It.IsAny<decimal>()),
            Times.Never);
        _notificationManagerMock.Verify(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }
}