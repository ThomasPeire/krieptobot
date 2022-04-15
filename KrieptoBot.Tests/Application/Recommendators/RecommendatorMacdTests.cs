using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Indicators.Results;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.Application.Recommendators
{
    public class RecommendatorMacdTests
    {
        private Mock<IExchangeService> _exchangeServiceMock;
        private Mock<ILogger<RecommendatorMacd>> _logger;
        private Mock<IOptions<RecommendatorSettings>> _recommendatorSettingOptions;
        private Mock<IMacd> _macdIndicator;
        private TradingContext _tradingContext;

        [SetUp]
        public void Setup()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _macdIndicator = new Mock<IMacd>();
            _logger = new Mock<ILogger<RecommendatorMacd>>();
            _recommendatorSettingOptions = new Mock<IOptions<RecommendatorSettings>>();
            _recommendatorSettingOptions.Setup(x => x.Value).Returns(new RecommendatorSettings
            {
                BuyRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorMacd), 1m }
                },
                SellRecommendationWeights = new Dictionary<string, decimal>
                {
                    { nameof(RecommendatorMacd), 1m }
                }
            });

            _tradingContext = new TradingContext(new DateTimeProvider(_exchangeServiceMock.Object, new Mock<IMemoryCache>().Object))
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
        public async Task RecommendationMacd_ShouldReturn_NegativeScoreWhenGoesBelowZero()
        {
            var macdResults = new MacdResult()
            {
                Histogram = new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, -10 },
                    { DateTime.Today.AddDays(-1), 5 },
                    { DateTime.Today.AddDays(-2), 5 }
                },

                MacdLine = new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 10 },
                    { DateTime.Today.AddDays(-1), 5 },
                    { DateTime.Today.AddDays(-2), 5 }
                }
            };

            var ema = new Mock<IExponentialMovingAverage>();
            _macdIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>()))
                .Returns(macdResults);

            var recommendator = new RecommendatorMacd(_recommendatorSettingOptions.Object, _logger.Object,
                _macdIndicator.Object, _exchangeServiceMock.Object,
                _tradingContext, ema.Object);

            var result =
                await recommendator.GetRecommendation(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero));

            Assert.That(result.Value, Is.LessThan(0));
        }

        [Test]
        public async Task RecommendationMacd_ShouldReturn_PositiveScoreWhenGoesAboveZero()
        {
            var macdResults = new MacdResult()
            {
                Histogram = new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, 10 },
                    { DateTime.Today.AddDays(-1), -5 },
                    { DateTime.Today.AddDays(-2), -5 }
                },

                MacdLine = new Dictionary<DateTime, decimal>
                {
                    { DateTime.Today, -10 },
                    { DateTime.Today.AddDays(-1), -5 },
                    { DateTime.Today.AddDays(-2), -5 }
                }
            };

            var ema = new Mock<IExponentialMovingAverage>();
            _macdIndicator
                .Setup(x => x.Calculate(It.IsAny<IEnumerable<Candle>>()))
                .Returns(macdResults);

            var recommendator = new RecommendatorMacd(_recommendatorSettingOptions.Object, _logger.Object,
                _macdIndicator.Object, _exchangeServiceMock.Object,
                _tradingContext, ema.Object);

            var result =
                await recommendator.GetRecommendation(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero));

            Assert.That(result.Value, Is.GreaterThan(0));
        }
    }
}
