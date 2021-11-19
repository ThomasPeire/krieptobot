using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.Entity;
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
            var priceToSellOn = await GetPriceToSellOn(market);
            var availableBaseAssetBalance = await GetBalanceAvailableToSell(market);

            LogSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);
            await SendNotificationWithSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);

            if (ShouldPlaceOrder())
                await PlaceOrder(market, availableBaseAssetBalance, priceToSellOn);
        }

        private async Task<Order> PlaceOrder(Market market, decimal availableBaseAssetBalance,
            TickerPrice priceToSellOn)
        {
            return await _exchangeService.PostSellOrderAsync(market.Name, "limit", availableBaseAssetBalance,
                priceToSellOn.Price);
        }

        private bool ShouldPlaceOrder()
        {
            return !_tradingContext.IsSimulation;
        }

        private async Task SendNotificationWithSellRecommendation(Market market, TickerPrice priceToSellOn,
            decimal availableBaseAssetBalance)
        {
            await _notificationManager.SendNotification(
                $"Selling on {market.Name.Value}.",
                $"Price: € {priceToSellOn.Price.Value} - Amount: {availableBaseAssetBalance} - € {availableBaseAssetBalance * priceToSellOn.Price:0.00}");
        }

        private void LogSellRecommendation(Market market, TickerPrice priceToSellOn, decimal availableBaseAssetBalance)
        {
            _logger.LogInformation("Selling on {Market}. Price: € {Price}; Amount: {Amount}; € {Euro}",
                market.Name.Value,
                priceToSellOn.Price.Value, availableBaseAssetBalance,
                (availableBaseAssetBalance * priceToSellOn.Price).ToString("0.00"));
        }

        private async Task<decimal> GetBalanceAvailableToSell(Market market)
        {
            var balance = await GetBalanceAsync(market);
            var availableBaseAssetBalance = 0m;

            if (balance != null)
                availableBaseAssetBalance = balance.Available - balance.InOrder;

            return availableBaseAssetBalance;
        }

        private async Task<Balance> GetBalanceAsync(Market market)
        {
            var balances = await _exchangeService.GetBalanceAsync();
            return balances.FirstOrDefault(x => x.Symbol == market.Name.BaseSymbol);
        }

        private async Task<TickerPrice> GetPriceToSellOn(Market market)
        {
            return await _exchangeService.GetTickerPrice(market.Name);
        }
    }
}
