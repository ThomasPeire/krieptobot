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
        private Mock<ILogger<Trader>> _logger;

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
        public async Task Trader_Should_SellWhenNeeded()
        {
            _recommendationCalculator
                .Setup(x => x.CalculateRecommendation(It.IsAny<Market>()))
                .Returns(Task.FromResult(new RecommendatorScore(-99)));

            var trader = new Trader(_logger.Object, _tradingContext, _exchangeServiceMock.Object,
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object);

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
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object);

            await trader.Run();

            _sellManagerMock.Verify(x => x.Sell(It.IsAny<Market>()), Times.Never);
            _buyManagerMock.Verify(x => x.Buy(It.IsAny<Market>(), It.IsAny<decimal>()), Times.Once);
        }


        [Test]
        public async Task Trader_Should_DivideBudgetWhenBuying()
        {
            var localTradingContext = new TradingContext(new DateTimeProvider())
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
                _recommendationCalculator.Object, _sellManagerMock.Object, _buyManagerMock.Object);

            await trader.Run();


            _buyManagerMock.Verify(x => x.Buy(new Market(new MarketName("DOGE-EUR"), Amount.Zero, Amount.Zero), 70),
                Times.Once);
            _buyManagerMock.Verify(x => x.Buy(new Market(new MarketName("BTC-EUR"), Amount.Zero, Amount.Zero), 130),
                Times.Once);
        }
    }
}
