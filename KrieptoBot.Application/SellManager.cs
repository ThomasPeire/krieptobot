using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class SellManager : ISellManager
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<SellManager> _logger;
        private readonly INotificationManager _notificationManager;
        private readonly ITradingContext _tradingContext;

        public SellManager(ILogger<SellManager> logger, ITradingContext tradingContext,
            INotificationManager notificationManager, IExchangeService exchangeService)
        {
            _logger = logger;
            _tradingContext = tradingContext;
            _notificationManager = notificationManager;
            _exchangeService = exchangeService;
        }

        public async Task Sell(Market market)
        {
            var tickerPrice = await _exchangeService.GetTickerPrice(market.Name);

            var balances = await _exchangeService.GetBalanceAsync();
            var baseAssetBalance = balances.FirstOrDefault(x => x.Symbol == market.BaseSymbol);
            var availableBaseAssetBalance = 0m;

            if (baseAssetBalance != null)
                availableBaseAssetBalance = baseAssetBalance.Available - baseAssetBalance.InOrder;

            //todo: take fees into account
            //todo: take min sell amount into account
            _logger.LogInformation("Selling on {Market}. Price: € {Price}; Amount: {Amount}; € {Euro}",
                market.Name,
                tickerPrice.Price.Value, availableBaseAssetBalance,
                (availableBaseAssetBalance * tickerPrice.Price).ToString("0.00"));

            await _notificationManager.SendNotification(
                $"Selling on {market.Name}.",
                $"Price: € {tickerPrice.Price.Value} - Amount: {availableBaseAssetBalance} - € {(availableBaseAssetBalance * tickerPrice.Price):0.00}");

            if (!_tradingContext.IsSimulation)
                await _exchangeService.PostSellOrderAsync(market.Name, "limit", availableBaseAssetBalance,
                    tickerPrice.Price);
        }
    }
}
