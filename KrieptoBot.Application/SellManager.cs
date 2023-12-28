﻿using System.Threading;
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
            await CancelOpenOrders(market);
            var availableBaseAssetBalance = await GetSellableBalance(market);

            var priceToSellOn = await _exchangeService.GetTickerPrice(market.Name.Value);
            LogSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);
            await SendNotificationWithSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);

            if (ShouldPlaceOrder())
            {
                await PlaceOrder(market, availableBaseAssetBalance, priceToSellOn);
            }
        }

        private async Task CancelOpenOrders(Market market)
        {
            await _exchangeService.CancelOrders(market.Name.Value);
        }

        private async Task PlaceOrder(Market market, decimal availableBaseAssetBalance, TickerPrice priceToSellOn)
        {
            await _exchangeService.PostSellOrderAsync(market.Name, OrderType.Limit, availableBaseAssetBalance, priceToSellOn.Price);
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

        private void LogSellRecommendation(Market market, TickerPrice priceToSellOn,
            decimal availableBaseAssetBalance)
        {
            _logger.LogInformation("Selling on {Market}. Price: € {Price}; Amount: {Amount}; € {Euro}",
                market.Name.Value,
                priceToSellOn.Price.Value, availableBaseAssetBalance,
                (availableBaseAssetBalance * priceToSellOn.Price).ToString("0.00"));
        }

        private async Task<decimal> GetSellableBalance(Market market)
        {
            var balance = await GetBalanceAsync(market);
            var sellableBalance = 0m;

            if (balance != null)
                sellableBalance = balance.Available + balance.InOrder;

            return sellableBalance;
        }

        private async Task<Balance> GetBalanceAsync(Market market)
        {
            return await _exchangeService.GetBalanceAsync(market.Name.BaseSymbol);
        }

        // private async Task<TickerPrice> GetPriceToSellOn(Market market)
        // {
        //     var lastCandles = await _exchangeService.GetCandlesAsync(market.Name, _tradingContext.Interval,
        //         end: _tradingContext.CurrentTime);
        //     var lastCandle = lastCandles.OrderByDescending(x => x.TimeStamp).First();
        //
        //     var bodyHigh = Math.Max(lastCandle.Close, lastCandle.Open);
        //     var bodyLow = Math.Min(lastCandle.Close, lastCandle.Open);
        //
        //     return new TickerPrice(market.Name, new Price(bodyHigh - (bodyHigh - bodyLow) / 2));
        // }
    }
}
