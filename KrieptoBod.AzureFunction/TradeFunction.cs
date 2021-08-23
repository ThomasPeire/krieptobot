using System.Diagnostics;
using KrieptoBod.Application;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KrieptoBod.AzureFunction
{
    public class TradeFunction
    {
        private readonly ITrader _trader;

        private const string ScheduleExpression = "2 */1 * * * *"; // 120 times an hour - at second 2 of every 30 seconds of every hour of each day
        //private const string ScheduleExpression = "2 */5 * * * *"; // 12 times an hour - at second 2 of every 5 minutes of every hour of each day

        public TradeFunction( ITrader trader)
        {
            _trader = trader;
        }

        [Function(nameof(TradeFunction))]
        public async Task Run([TimerTrigger(ScheduleExpression, RunOnStartup = true, UseMonitor = true)] TimerInfo myTimer)
        {
            await _trader.Run();

        }
    }
}