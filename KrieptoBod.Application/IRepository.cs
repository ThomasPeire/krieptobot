using KrieptoBod.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBod.Application
{
    public interface IRepository
    {
        Task<IEnumerable<Balance>> GetBalanceAsync();
        Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000,
            DateTime? start = null, DateTime? end = null);
        Task<IEnumerable<Market>> GetMarketsAsync();
        Task<Market> GetMarketAsync(string market);
        Task<IEnumerable<Asset>> GetAssetsAsync();
        Task<Asset> GetAssetAsync(string symbol);
        Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null);
        Task<Order> GetOrderAsync(string market, Guid orderId);
        Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null);
        Task<Order> GetOpenOrderAsync();
        Task<Order> GetOpenOrderAsync(string market);
    }
}
