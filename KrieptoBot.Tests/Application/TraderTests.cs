using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Application.Recommendators;
using KrieptoBot.Application.Settings;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private Mock<ILogger<Trader>> _logger;
        private TradingSettings _tradingSettings;

        [SetUp]
        public void Setup()
        {
            _buyManagerMock = new Mock<IBuyManager>();
            _sellManagerMock = new Mock<ISellManager>();
            _recommendationCalculator = new Mock<IRecommendationCalculator>();
            _exchangeServiceMock = new Mock<IExchangeService>();
            _logger = new Mock<ILogger<Trader>>();

            _exchangeServiceMock
                .Setup(x => x.GetMarketAsync("BTC-EUR"))
                .Returns(Task.FromResult(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero)));

            _tradingContext = new TradingContext(new DateTimeProvider(_exchangeServiceMock.Object,new Mock<IMemoryCache>().Object))
                .SetBuyMargin(30)
                .SetSellMargin(-30)
                .SetMarketsToWatch(
                    new List<string>
                    {
                        "BTC-EUR"
                    })
                .SetInterval("5m");

            _tradingSettings = new TradingSettings { MaxBuyBudgetPerCoin = 100m, MinBuyBudgetPerCoin = 0m };
        }

        [Test]
        public async Task Trader_Should_SellWhenNeeded()
        {
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(-99)));

            var trader = new Trader(_logger.Object, _tradingContext, _exchangeServiceMock.Object,
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object,
                new OptionsWrapper<TradingSettings>(_tradingSettings));

            await trader.Run();

            _sellManagerMock.Verify(x => x.Sell(It.IsAny<Market>()), Times.Once);
            _buyManagerMock.Verify(x => x.Buy(It.IsAny<Market>(), It.IsAny<decimal>()), Times.Never);
        }

        [Test]
        public async Task Trader_Should_BuyWhenNeeded()
        {
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(+99)));

            var trader = new Trader(_logger.Object, _tradingContext, _exchangeServiceMock.Object,
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object,
                new OptionsWrapper<TradingSettings>(_tradingSettings));

            await trader.Run();

            _sellManagerMock.Verify(x => x.Sell(It.IsAny<Market>()), Times.Never);
            _buyManagerMock.Verify(x => x.Buy(It.IsAny<Market>(), It.IsAny<decimal>()), Times.Once);
        }


        [Test]
        public async Task Trader_Should_DivideBudgetWhenBuying()
        {
            var iExchangeServiceMock = new Mock<IExchangeService>();
            var localTradingContext = new TradingContext(new DateTimeProvider(iExchangeServiceMock.Object, new Mock<IMemoryCache>().Object))
                .SetBuyMargin(30)
                .SetSellMargin(-30)
                .SetMarketsToWatch(
                    new List<string>
                    {
                        "BTC-EUR", "DOGE-EUR"
                    })
                .SetInterval("5m");

            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero)))
                .Returns(Task.FromResult(new RecommendatorScore(65)));

            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(new Market(new MarketName("DOGE-EUR"), Amount.Zero, Amount.Zero)))
                .Returns(Task.FromResult(new RecommendatorScore(35)));

            _exchangeServiceMock
                .Setup(x => x.GetBalanceAsync("EUR"))
                .Returns(Task.FromResult(new Balance(new Symbol("EUR"), new Amount(200), Amount.Zero)));

            _exchangeServiceMock
                .Setup(x => x.GetMarketAsync("DOGE-EUR"))
                .Returns(Task.FromResult(new Market(new MarketName("DOGE-EUR"), Amount.Zero, Amount.Zero)));

            var trader = new Trader(_logger.Object, localTradingContext, _exchangeServiceMock.Object,
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object,
                new OptionsWrapper<TradingSettings>(_tradingSettings));

            await trader.Run();


            _buyManagerMock.Verify(
                x => x.Buy(new Market(new MarketName("DOGE-EUR"), Amount.Zero, Amount.Zero),
                    Math.Min(_tradingSettings.MaxBuyBudgetPerCoin, 66)),
                Times.Once);
            _buyManagerMock.Verify(
                x => x.Buy(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero),
                    Math.Min(_tradingSettings.MaxBuyBudgetPerCoin, 123)),
                Times.Once);
        }
    }
}
