using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class SellManager : ISellManager
    {
        private readonly ILogger<SellManager> _logger;

        public SellManager(ILogger<SellManager> logger)
        {
            _logger = logger;
        }
        public async Task Sell(string market)
        {
            _logger.LogDebug("Selling on {Market}", market);
        }
    }
}
