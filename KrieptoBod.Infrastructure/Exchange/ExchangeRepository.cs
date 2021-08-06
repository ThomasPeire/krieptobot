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

        public Task<IEnumerable<Market>> GetMarketsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Market> GetMarketAsync(string market)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Asset> GetAssetAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOpenOrderAsync(string market = "")
        {
            throw new NotImplementedException();
        }
    }
}
