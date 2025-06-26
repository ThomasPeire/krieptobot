using System.Threading;
using KrieptoBot.Application;
using KrieptoBot.AzureFunction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.AzureFunction;

public class TradeFunctionTests
{
    [Test]
    public void TradeFunction_Should_RunTrader()
    {
        var mockTrader = new Mock<ITrader>();
        var mockTradingContext = new Mock<ITradingContext>();
        var logger = new Mock<ILogger<TradeFunction>>();

        var tradeFunction = new TradeFunction(mockTrader.Object, logger.Object, mockTradingContext.Object);

        _ = tradeFunction.Run(new TimerInfo());

        mockTrader.Verify(x => x.Run(CancellationToken.None), Times.Once);
    }
}