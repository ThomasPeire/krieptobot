using System;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.ConsoleLauncher.Tests.ConsoleLauncher
{
    public class TradeServiceTests
    {
        private Mock<ITrader> _mockTrader = new();
        private Mock<IDateTimeProvider> _mockDateTimeProvider = new();
        private Mock<ITradingContext> _mockTradingContext = new();
        private Mock<ILogger<TradeService>> _mockLogger = new();

        [SetUp]
        public void Setup()
        {
            _mockTradingContext.Setup(x => x.PollingIntervalInMinutes).Returns(5);
        }


        [Test]
        public async Task TradeService_Should_RunTrader()
        {
            _mockDateTimeProvider.Setup(x => x.UtcDateTimeNowSyncedWithExchange())
                .Returns(Task.FromResult(new DateTime(2001, 1, 1, 10, 0, 0)));
            _mockDateTimeProvider.Setup(x => x.UtcDateTimeNowExchange())
                .Returns(Task.FromResult(new DateTime(2001, 1, 1, 10, 0, 0)));
            _mockTradingContext.Setup(x => x.Interval).Returns(Interval.FiveMinutes);

            var tradeService = new TradeService(_mockLogger.Object, _mockTrader.Object, _mockTradingContext.Object,
                _mockDateTimeProvider.Object);

            await tradeService.StartAsync(new CancellationToken());
            _mockTrader.Verify(x => x.Run(), Times.Once);
        }

        [Test]
        public void TradeServiceStopAsync_Should_ReturnTaskComplete()
        {
            var tradeService = new TradeService(_mockLogger.Object, _mockTrader.Object, _mockTradingContext.Object,
                _mockDateTimeProvider.Object);

            var result = tradeService.StopAsync(new CancellationToken());

            Assert.That(result, Is.EqualTo(Task.CompletedTask));
        }
    }
}
