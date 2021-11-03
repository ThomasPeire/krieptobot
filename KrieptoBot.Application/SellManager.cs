﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class SellManager : ISellManager
    {
        private readonly ILogger<SellManager> _logger;
        private readonly INotificationManager _notificationManager;

        public SellManager(ILogger<SellManager> logger, INotificationManager notificationManager)
        {
            _logger = logger;
            _notificationManager = notificationManager;
        }
        public async Task Sell(string market)
        {
            _logger.LogDebug("Selling on {Market}", market);
            await _notificationManager.SendNotification($"Selling on {market}");
        }
    }
}
