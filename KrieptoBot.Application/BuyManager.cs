using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class BuyManager : IBuyManager
    {
        private readonly ILogger<BuyManager> _logger;

        public BuyManager(ILogger<BuyManager> logger)
        {
            _logger = logger;
        }

        public async Task Buy(string market, float budget)
        {
            _logger.LogDebug("Buying on {Market} with budget {Budget}", market, budget);
        }
    }
}
