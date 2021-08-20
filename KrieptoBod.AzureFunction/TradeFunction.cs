using KrieptoBod.Application;
using KrieptoBod.Exchange.Bitvavo;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KrieptoBod.AzureFunction
{
    public class TradeFunction
    {
        private readonly ITrader _trader;
        private readonly ILogger _logger;

        private const string ScheduleExpression = "2 */1 * * * *"; // 120 times an hour - at second 2 of every 30 seconds of every hour of each day
        //private const string ScheduleExpression = "2 */5 * * * *"; // 12 times an hour - at second 2 of every 5 minutes of every hour of each day

        public TradeFunction(ILogger<Startup> logger, ITrader trader)
        {
            _trader = trader;
            _logger = logger;
        }

        [FunctionName("TradeFunction")]
        public async Task Run([TimerTrigger(ScheduleExpression, RunOnStartup = true)] TimerInfo myTimer)
        {
            await _trader.Run();
        }

        
    }
}