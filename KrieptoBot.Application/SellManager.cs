using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class SellManager : ISellManager
    {
        private readonly ILogger<SellManager> _logger;
        private readonly ITradingContext _tradingContext;
        private readonly INotificationManager _notificationManager;
        private readonly IExchangeService _exchangeService;

        public SellManager(ILogger<SellManager> logger, ITradingContext tradingContext,
            INotificationManager notificationManager, IExchangeService exchangeService)
        {
            _logger = logger;
            _tradingContext = tradingContext;
            _notificationManager = notificationManager;
            _exchangeService = exchangeService;
        }

        public async Task Sell(string market)
        {
            var tickerPrice = await _exchangeService.GetTickerPrice(market);
            var baseAsset = market[0..market.IndexOf("-", StringComparison.Ordinal)];

            var balances = await _exchangeService.GetBalanceAsync();
            var baseAssetBalance = balances.FirstOrDefault(x => x.Symbol == baseAsset);
            var availableBaseAssetBalance = 0m;

            if (baseAssetBalance != null)
                availableBaseAssetBalance = baseAssetBalance.Available - baseAssetBalance.InOrder;

            //todo: take fees into account
            _logger.LogInformation("Selling on {Market}. Price: € {Price}; Amount: {Amount}; € {Euro}", market,
                tickerPrice.Price, availableBaseAssetBalance, availableBaseAssetBalance * tickerPrice.Price);

            await _notificationManager.SendNotification(
                $"Selling on {market}.", $"Price: € {tickerPrice.Price} - Amount: {availableBaseAssetBalance} - € {availableBaseAssetBalance * tickerPrice.Price}");

            if (!_tradingContext.IsSimulation)
            {
                await _exchangeService.PostSellOrderAsync(market, "limit", (double)availableBaseAssetBalance, (double)tickerPrice.Price);
            }
        }
    }
}
