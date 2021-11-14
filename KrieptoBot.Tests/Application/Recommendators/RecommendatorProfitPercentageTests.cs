using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorProfitPercentageTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<ILogger<RecommendatorProfitPercentage>> _logger;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _logger = new Mock<ILogger<RecommendatorProfitPercentage>>();

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
        public async Task Recommendation_ShouldReturn_NegativeScoreWhenProfit()
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object);

            _exchangeServiceMock
                .Setup(x => x.GetTradesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Trade>
                    {
                        new(Guid.Empty, DateTime.Now.AddDays(-10), new Market("btc-eur"),
                            new Amount(10), new Price(4), OrderSide.Sell),
                        new(Guid.Empty, DateTime.Now.AddDays(-9), new Market("btc-eur"),
                            new Amount(4), new Price(3), OrderSide.Buy),
                        new(Guid.Empty, DateTime.Now.AddDays(-9), new Market("btc-eur"),
                            new Amount(4), new Price(4), OrderSide.Buy)
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice(new Market("btc-eur"), new Price(4)));


            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.LessThan(0m));
        }


        [TestCase(3)]
        [TestCase(2)]
        public async Task Recommendation_ShouldReturn_ZeroWhenNoProfitOrBreakEven(decimal tickerPrice)
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object);

            _exchangeServiceMock
                .Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Order>
                    {
                        new(Guid.Empty, new Market("btc-eur"), DateTime.Now.AddDays(-10),
                            DateTime.Now.AddDays(-10), OrderStatus.Filled, OrderSide.Sell, OrderType.Limit,
                            new Amount(1), new Price(4)),
                        new(Guid.Empty, new Market("btc-eur"), DateTime.Now.AddDays(-9),
                            DateTime.Now.AddDays(-9), OrderStatus.Filled, OrderSide.Buy, OrderType.Limit,
                            new Amount(1), new Price(3))
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice(new Market("btc-eur"), new Price(tickerPrice)));


            var result = await recommendator.GetRecommendation(new Market("BTC-EUR"));

            Assert.That(result.Value, Is.EqualTo(0m));
        }
    }
}
