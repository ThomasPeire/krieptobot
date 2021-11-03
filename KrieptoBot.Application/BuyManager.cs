using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class BuyManager : IBuyManager
    {
        private readonly ILogger<BuyManager> _logger;
        private readonly INotificationManager _notificationManager;

        public BuyManager(ILogger<BuyManager> logger, INotificationManager notificationManager)
        {
            _logger = logger;
            _notificationManager = notificationManager;
        }

        public async Task Buy(string market, float budget)
        {
            _logger.LogDebug("Buying on {Market} with budget {Budget}", market, budget);
            await _notificationManager.SendNotification($"Buying on {market} with budget {budget}");
        }
    }
}
