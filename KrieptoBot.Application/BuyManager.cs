using System.Threading.Tasks;
using KrieptoBot.Model;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class BuyManager : IBuyManager
    {
        private readonly ILogger<BuyManager> _logger;
        private readonly ITradingContext _tradingContext;
        private readonly INotificationManager _notificationManager;
        private readonly IExchangeService _exchangeService;

        public BuyManager(ILogger<BuyManager> logger, ITradingContext tradingContext,
            INotificationManager notificationManager,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _tradingContext = tradingContext;
            _notificationManager = notificationManager;
            _exchangeService = exchangeService;
        }

        public async Task Buy(Market market, decimal budget)
        {
            var tickerPrice = await _exchangeService.GetTickerPrice(market.MarketName);
            var amount = budget / tickerPrice.Price;

            //todo: take fees into account
            //todo: take min buy amount into account
            _logger.LogInformation("Buying on {Market} with € {Budget}. Price: € {Price}; Amount: {Amount}",
                market.MarketName,
                budget, tickerPrice.Price, amount);
            await _notificationManager.SendNotification($"Buying on {market.MarketName} with € {budget}",
                $"Price: € {tickerPrice.Price}; Amount: {amount}");

            if (!_tradingContext.IsSimulation)
            {
                await _exchangeService.PostBuyOrderAsync(market.MarketName, "limit", amount,
                    tickerPrice.Price);
            }
        }
    }
}
