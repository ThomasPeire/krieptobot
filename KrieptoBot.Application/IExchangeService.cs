﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Domain;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application;

public interface IExchangeService
{
    Task<IEnumerable<Balance>> GetBalanceAsync();

    Task<Balance> GetBalanceAsync(string symbol);

    Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = Interval.Minutes.Five, int limit = 1000,
        DateTime? start = null, DateTime? end = null, CancellationToken ct = default);

    Task<IEnumerable<Market>> GetMarketsAsync();
    Task<Market> GetMarketAsync(string market);
    Task<IEnumerable<Asset>> GetAssetsAsync();
    Task<Asset> GetAssetAsync(string symbol);

    Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null,
        DateTime? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null);

    Task<Order> GetOrderAsync(string market, Guid orderId);

    Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null,
        DateTime? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null);

    Task<IEnumerable<Order>> GetOpenOrderAsync();
    Task<IEnumerable<Order>> GetOpenOrderAsync(string market);
    Task<Order> PostSellOrderAsync(string market, string orderType, decimal amount, decimal price = default);
    Task<Order> PostStopLossOrderAsync(string market, decimal amount, decimal price);
    Task<Order> PostBuyOrderAsync(string market, string orderType, decimal amount, decimal price);
    Task<TickerPrice> GetTickerPrice(string market);
    Task CancelOrders(string market = "");
    Task CancelOrder(string market, Guid orderId);

    Task<DateTime> GetTime();
    Task UpdateOrderPrice(string market, Guid id, decimal price);
    Task UpdateOrderTriggerAmount(string market, Guid id, decimal price);
}