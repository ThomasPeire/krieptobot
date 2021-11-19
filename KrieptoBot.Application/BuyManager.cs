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
            var priceToBuyOn = await GetPriceToBuyOn(market);
            var amount = GetAmountToBuy(budget, priceToBuyOn);

            LogBuyRecommendation(market, budget, priceToBuyOn, amount);
            await SendNotificationWithBuyRecommendation(market, budget, priceToBuyOn, amount);

            if (ShouldPlaceOrder())
                await PlaceOrder(market, amount, priceToBuyOn);
        }

        private async Task PlaceOrder(Market market, decimal amount, TickerPrice priceToBuyOn)
        {
            await _exchangeService.PostBuyOrderAsync(market.Name, "limit", amount,
                priceToBuyOn.Price);
        }

        private bool ShouldPlaceOrder()
        {
            return !_tradingContext.IsSimulation;
        }

        private async Task SendNotificationWithBuyRecommendation(Market market, decimal budget,
            TickerPrice priceToBuyOn,
            decimal amount)
        {
            await _notificationManager.SendNotification($"Buying on {market.Name.Value} with € {budget}",
                $"Price: € {priceToBuyOn.Price.Value}; Amount: {amount}");
        }

        private void LogBuyRecommendation(Market market, decimal budget, TickerPrice priceToBuyOn, decimal amount)
        {
            _logger.LogInformation("Buying on {Market} with € {Budget}. Price: € {Price}; Amount: {Amount}",
                market.Name.Value,
                budget, priceToBuyOn.Price.Value, amount);
        }

        private static decimal GetAmountToBuy(decimal budget, TickerPrice priceToBuyOn)
        {
            return budget / priceToBuyOn.Price;
        }

        private async Task<TickerPrice> GetPriceToBuyOn(Market market)
        {
            return await _exchangeService.GetTickerPrice(market.Name);
        }
    }
}
