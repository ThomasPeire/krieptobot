using System.Globalization;
using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application
{
    public class BuyManager : IBuyManager
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<BuyManager> _logger;
        private readonly INotificationManager _notificationManager;
        private readonly ITradingContext _tradingContext;

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
            var tickerPrice = await _exchangeService.GetTickerPrice(market.Name);
            var amount = budget / tickerPrice.Price;

            //todo: take fees into account
            //todo: take min buy amount into account
            _logger.LogInformation("Buying on {Market} with € {Budget}. Price: € {Price}; Amount: {Amount}",
                market.Name,
                budget, tickerPrice.Price.Value, amount);
            await _notificationManager.SendNotification($"Buying on {market.Name} with € {budget}",
                $"Price: € {tickerPrice.Price.Value}; Amount: {amount}");

            if (!_tradingContext.IsSimulation)
                await _exchangeService.PostBuyOrderAsync(market.Name, "limit", amount,
                    tickerPrice.Price);
        }
    }
}
