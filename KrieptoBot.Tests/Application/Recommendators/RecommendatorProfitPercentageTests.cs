using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorProfitPercentageTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<ILogger<RecommendatorProfitPercentage>> _logger;
        private Mock<IOptions<RecommendatorSettings>> _recommendatorSettingOptions;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _logger = new Mock<ILogger<RecommendatorProfitPercentage>>();
            _recommendatorSettingOptions = new Mock<IOptions<RecommendatorSettings>>();

            _recommendatorSettingOptions.Setup(x => x.Value).Returns(new RecommendatorSettings
            {
                BuyRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorProfitPercentage), 1m }
                },
                SellRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorProfitPercentage), 1m }
                }
            });

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
        public async Task Recommendation_ShouldReturn_NegativeScoreWhenProfit()
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object,
                _recommendatorSettingOptions.Object);

            _exchangeServiceMock
                .Setup(x => x.GetTradesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Trade>
                    {
                        new(Guid.Empty, DateTime.Now.AddDays(-10), new MarketName("btc-eur"),
                            new Amount(10), new Price(4), OrderSide.Sell),
                        new(Guid.Empty, DateTime.Now.AddDays(-9), new MarketName("btc-eur"),
                            new Amount(4), new Price(3), OrderSide.Buy),
                        new(Guid.Empty, DateTime.Now.AddDays(-9), new MarketName("btc-eur"),
                            new Amount(4), new Price(4), OrderSide.Buy)
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice(new MarketName("btc-eur"), new Price(4)));


            var result =
                await recommendator.GetRecommendation(new Market(new MarketName("btc-eur"), Amount.Zero, Amount.Zero));

            Assert.That(result.Value, Is.LessThan(0m));
        }


        [Test]
        public async Task Recommendation_ShouldReturn_NegativeScoreWhenNoProfit()
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object,
                _recommendatorSettingOptions.Object);

            _exchangeServiceMock
                .Setup(x => x.GetTradesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Trade>
                    {
                        new(Guid.Empty, DateTime.Now.AddDays(-10), new MarketName("btc-eur"),
                            new Amount(1), new Price(4), OrderSide.Sell),
                        new(Guid.Empty, DateTime.Now.AddDays(-10), new MarketName("btc-eur"),
                            new Amount(1), new Price(3), OrderSide.Buy)
                    });

            _exchangeServiceMock
                .Setup(x => x.GetTickerPrice(It.IsAny<string>())).ReturnsAsync(
                    new TickerPrice(new MarketName("btc-eur"), new Price(2)));

            var result =
                await recommendator.GetRecommendation(new Market(new MarketName("btc-eur"), Amount.Zero, Amount.Zero));

            Assert.That(result.Value, Is.LessThan(0m));
        }

        [Test]
        public async Task Recommendation_ShouldReturnRecommendationZeroAndExcluded_WhenNoBuyTradesWereFound()
        {
            var recommendator = new RecommendatorProfitPercentage(_logger.Object, _exchangeServiceMock.Object,
                _recommendatorSettingOptions.Object);

            _exchangeServiceMock
                .Setup(x => x.GetTradesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>())).ReturnsAsync(
                    new List<Trade>
                    {
                        new(Guid.Empty, DateTime.Now.AddDays(-10), new MarketName("btc-eur"),
                            new Amount(1), new Price(4), OrderSide.Sell)
                    });

            var result =
                await recommendator.GetRecommendation(new Market(new MarketName("btc-eur"), Amount.Zero, Amount.Zero));

            Assert.That(result.Value, Is.EqualTo(0m));
            Assert.That(result.IncludeInAverageScore, Is.False);
        }
    }
}
