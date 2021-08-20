using KrieptoBod.Application;
using KrieptoBod.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBod.Infrastructure.Exchange
{
    public class ExchangeRepository : IRepository
    {
        private readonly IExchangeService _service;

        public ExchangeRepository(IExchangeService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            return await _service.GetBalanceAsync();
        }

        public async Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, DateTime? start = null,
            DateTime? end = null)
        {
            return await _service.GetCandlesAsync(market, interval, limit, start, end);
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            return await _service.GetMarketsAsync();
        }

        public async Task<Market> GetMarketAsync(string market)
        {
            return await _service.GetMarketAsync(market);
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            return await _service.GetAssetsAsync();
        }

        public async Task<Asset> GetAssetAsync(string symbol)
        {
            return await _service.GetAssetAsync(symbol);
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            return await _service.GetTradesAsync(market, limit, start, end, tradeIdFrom, tradeIdTo);
        }

        public async Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            return await _service.GetOrderAsync(market, orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            return await _service.GetOrdersAsync(market, limit, start, end, orderIdFrom, orderIdTo);
        }

        public async Task<Order> GetOpenOrderAsync()
        {
            return await GetOpenOrderAsync("");
        }
        public async Task<Order> GetOpenOrderAsync(string market)
        {
            return await _service.GetOpenOrderAsync(market);
        }
    }
}
