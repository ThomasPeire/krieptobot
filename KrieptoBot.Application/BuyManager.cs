using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class BuyManager : IBuyManager
    {
        private readonly ILogger<BuyManager> _logger;
        private readonly ITradingContext _tradingContext;
        private readonly INotificationManager _notificationManager;
        private readonly IExchangeService _exchangeService;

        public BuyManager(ILogger<BuyManager> logger, ITradingContext tradingContext, INotificationManager notificationManager,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _tradingContext = tradingContext;
            _notificationManager = notificationManager;
            _exchangeService = exchangeService;
        }

        public async Task Buy(string market, float budget)
        {
            var tickerPrice = await _exchangeService.GetTickerPrice(market);
            var amount = (decimal)budget / tickerPrice.Price;

            //todo: take fees into account
            _logger.LogInformation("Buying on {Market} with € {Budget}. Price: € {Price}; Amount: {Amount}", market,
                budget, tickerPrice.Price, amount);
            await _notificationManager.SendNotification($"Buying on {market} with € {budget}", $"Price: € {tickerPrice.Price}; Amount: {amount}");

            if(!_tradingContext.IsSimulation)
            {
                await _exchangeService.PostBuyOrderAsync(market, "limit", (double)amount, (double)tickerPrice.Price);
            }
        }
    }
}
