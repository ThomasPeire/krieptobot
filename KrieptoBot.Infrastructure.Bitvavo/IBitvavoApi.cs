using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [Get("/v2/{market}/candles")]
        Task<IEnumerable<JArray>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, long? start = null, long? end = null);
        
        [Get("/v2/markets")]
        Task<MarketDto> GetMarketAsync(string market);

        [Get("/v2/markets")]
        Task<IEnumerable<MarketDto>> GetMarketsAsync();

        [Get("/v2/{market}/trades")]
        Task<IEnumerable<TradeDto>> GetTradesAsync(string market, int limit = 500, long? start = null, long? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null);

        [Get("/v2/orders")]
        Task<IEnumerable<OrderDto>> GetOrdersAsync(string market, int limit = 500, long? start = null, long? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null);

        [Get("/v2/order")]
        Task<OrderDto> GetOrderAsync(string market, Guid orderId);

        [Get("/v2/ordersOpen")]
        Task<OrderDto> GetOpenOrderAsync(string market = "");

        [Post("/v2/order")]
        Task<OrderDto> PostOrderAsync(string market, string side, string orderType, string amount, string price);
    }
}
