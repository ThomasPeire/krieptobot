using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application;

public class SellManager(
    ILogger<SellManager> logger,
    ITradingContext tradingContext,
    INotificationManager notificationManager,
    IExchangeService exchangeService)
    : ISellManager
{
    public async Task Sell(Market market)
    {
        var availableBaseAssetBalance = await GetSellableBalance(market);

        var priceToSellOn = await exchangeService.GetTickerPrice(market.Name.Value);
        LogSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);
        await SendNotificationWithSellRecommendation(market, priceToSellOn, availableBaseAssetBalance);

        if (ShouldPlaceOrder())
        {
            await CancelOpenOrders(market);
            await PlaceOrder(market, availableBaseAssetBalance, priceToSellOn);
        }
    }

    private async Task CancelOpenOrders(Market market)
    {
        await exchangeService.CancelOrders(market.Name.Value);
    }

    private async Task PlaceOrder(Market market, decimal availableBaseAssetBalance, TickerPrice priceToSellOn)
    {
        await exchangeService.PostSellOrderAsync(market.Name, OrderType.Limit, availableBaseAssetBalance,
            priceToSellOn.Price);
    }

    private bool ShouldPlaceOrder()
    {
        return !tradingContext.IsSimulation;
    }

    private async Task SendNotificationWithSellRecommendation(Market market, TickerPrice priceToSellOn,
        decimal availableBaseAssetBalance)
    {
        await notificationManager.SendNotification(
            $"Selling on {market.Name.Value}.",
            $"Price: € {priceToSellOn.Price.Value} - Amount: {availableBaseAssetBalance} - € {availableBaseAssetBalance * priceToSellOn.Price:0.00}");
    }

    private void LogSellRecommendation(Market market, TickerPrice priceToSellOn,
        decimal availableBaseAssetBalance)
    {
        logger.LogInformation("Selling on {Market}. Price: € {Price}; Amount: {Amount}; € {Euro}",
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
        return await exchangeService.GetBalanceAsync(market.Name.BaseSymbol);
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