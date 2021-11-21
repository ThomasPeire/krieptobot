using KrieptoBot.Application;
using KrieptoBot.AzureFunction;
using Microsoft.Azure.Functions.Worker;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.AzureFunction
{
    public class TradeFunctionTests
    {
        [Test]
        public void TradeFunction_Should_RunTrader()
        {
            var mockTrader = new Mock<ITrader>();

            var tradeFunction = new TradeFunction(mockTrader.Object);

            tradeFunction.Run(new TimerInfo());

            mockTrader.Verify(x =>x.Run(), Times.Once);
        }
    }
}
