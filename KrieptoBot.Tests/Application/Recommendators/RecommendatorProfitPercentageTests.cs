using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KrieptoBot.Application.Recommendators;
using NUnit.Framework;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal.Commands;

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
                .Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Order>()
                    {
                        new()
                        {
                            Price = 4, Side = "sell", Amount = 10, Status = "filled",
                            Created = DateTime.Now.AddDays(-10)
                        },
                        new()
                        {
                            Price = 3, Side = "buy", Amount = 4, Status = "filled", Created = DateTime.Now.AddDays(-9)
                        },
                        new()
                        {
                            Price = 4, Side = "buy", Amount = 4, Status = "filled", Created = DateTime.Now.AddDays(-8)
                        },
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice() { Price = 4, Market = "" });


            var result = await recommendator.GetRecommendation(new Market { MarketName = "BTC-EUR" });

            Assert.That(result.Score, Is.LessThan(0m));
        }


        [TestCase(3)]
        [TestCase(2)]
        public async Task Recommendation_ShouldReturn_ZeroWhenNoProfitOrBreakEven(decimal tickerPrice)
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object);

            _exchangeServiceMock
                .Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Order>()
                    {
                        new()
                        {
                            Price = 4, Side = "sell", Amount = 1, Status = "filled", Created = DateTime.Now.AddDays(-10)
                        },
                        new()
                        {
                            Price = 3, Side = "buy", Amount = 1, Status = "filled", Created = DateTime.Now.AddDays(-9)
                        }
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice() { Price = tickerPrice, Market = "" });


            var result = await recommendator.GetRecommendation(new Market { MarketName = "BTC-EUR" });

            Assert.That(result.Score, Is.EqualTo(0m));
        }
    }
}
