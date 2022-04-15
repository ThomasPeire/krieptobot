using System.Threading.Tasks;
using KrieptoBot.Application;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.AzureFunction
{
    public class TradeFunction
    {
        private readonly ITrader _trader;
        private readonly ILogger<TradeFunction> _logger;
        private readonly ITradingContext _tradingContext;
        private const string ScheduleExpression = "5 */5 * * * *"; // 12 times an hour - at second 5 of every 5 minutes of every hour of each day

        public TradeFunction(ITrader trader, ILogger<TradeFunction> logger, ITradingContext tradingContext)
        {
            _trader = trader;
            _logger = logger;
            _tradingContext = tradingContext;
        }

        [Function(nameof(TradeFunction))]
        public async Task Run(
            [TimerTrigger(ScheduleExpression, RunOnStartup = false, UseMonitor = true)] TimerInfo myTimer)
        {
            _logger.LogDebug("Starting trading service");
            await _tradingContext.SetCurrentTime();
            await _trader.Run();
        }
    }
}
