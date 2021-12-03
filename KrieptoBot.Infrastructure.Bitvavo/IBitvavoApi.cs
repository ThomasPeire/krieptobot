﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using Newtonsoft.Json.Linq;
using Refit;

namespace KrieptoBot.Infrastructure.Bitvavo
{
    public interface IBitvavoApi
    {
        [Get("/v2/assets")]
        Task<AssetDto> GetAssetAsync(string symbol);

        [Get("/v2/assets")]
        Task<IEnumerable<AssetDto>> GetAssetsAsync();

        [Get("/v2/balance")]
        Task<IEnumerable<BalanceDto>> GetBalanceAsync();

        [Get("/v2/balance")]
        Task<BalanceDto> GetBalanceAsync(string symbol);

        [Get("/v2/{market}/candles")]
        Task<IEnumerable<JArray>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000,
            long? start = null, long? end = null);

        [Get("/v2/markets")]
        Task<MarketDto> GetMarketAsync(string market);

        [Get("/v2/markets")]
        Task<IEnumerable<MarketDto>> GetMarketsAsync();

        [Get("/v2/trades")]
        Task<IEnumerable<TradeDto>> GetTradesAsync(string market, int limit = 500, long? start = null, long? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null);

        [Get("/v2/orders")]
        Task<IEnumerable<OrderDto>> GetOrdersAsync(string market, int limit = 500, long? start = null, long? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null);

        [Get("/v2/order")]
        Task<OrderDto> GetOrderAsync(string market, Guid orderId);

        [Get("/v2/ordersOpen")]
        Task<IEnumerable<OrderDto>> GetOpenOrdersAsync(string market = "");

        [Post("/v2/order")]
        Task<OrderDto> PostOrderAsync(string market, string side, string orderType, string amount, string price);

        [Get("/v2/ticker/price")]
        Task<TickerPriceDto> GetTickerPrice(string market);
    }
}
