using System.Threading.Tasks;
using KrieptoBot.Application;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.AzureFunction
{
    public class TradeFunction(ITrader trader, ILogger<TradeFunction> logger, ITradingContext tradingContext)
    {
        private const string ScheduleExpression = "5 */5 * * * *"; // 12 times an hour - at second 5 of every 5 minutes of every hour of each day

        [Function(nameof(TradeFunction))]
        public async Task Run(
            [TimerTrigger(ScheduleExpression, RunOnStartup = false, UseMonitor = true)] TimerInfo myTimer)
        {
            logger.LogDebug("Starting trading service");
            await tradingContext.SetCurrentTime();
            await trader.Run();
        }
    }
}
