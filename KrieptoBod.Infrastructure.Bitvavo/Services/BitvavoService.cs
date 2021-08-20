using KrieptoBod.Application;
using KrieptoBod.Infrastructure.Bitvavo.Extensions;
using KrieptoBod.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrieptoBod.Infrastructure.Bitvavo
{
    public class BitvavoService : IExchangeService
    {
        private readonly IBitvavoApi _bitvavoApi;

        public BitvavoService(IBitvavoApi bitvavoApi)
        {
            _bitvavoApi = bitvavoApi;
        }

        public async Task<Asset> GetAssetAsync(string symbol)
        {
            var dto = await _bitvavoApi.GetAssetAsync(symbol);

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            var dtos = await _bitvavoApi.GetAssetsAsync();

            return dtos.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var dtos = await _bitvavoApi.GetBalanceAsync();

            return dtos.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, DateTime? start = null, DateTime? end = null)
        {
            var dtos = await _bitvavoApi.GetCandlesAsync(market, interval, limit, start, end);

            return dtos.ConvertToKrieptoBodModel();
        }

        public async Task<Market> GetMarketAsync(string market)
        {
            var dto = await _bitvavoApi.GetMarketAsync(market);

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            var dtos = await _bitvavoApi.GetMarketsAsync();

            return dtos.ConvertToKrieptoBodModel();
        }

        public async Task<Order> GetOpenOrderAsync()
        {
            var dto = await _bitvavoApi.GetOpenOrderAsync();

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<Order> GetOpenOrderAsync(string market)
        {
            var dto = await _bitvavoApi.GetOpenOrderAsync(market);

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            var dto = await _bitvavoApi.GetOrderAsync(market, orderId);

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            var dtos = await _bitvavoApi.GetOrdersAsync(market, limit, start, end, orderIdFrom, orderIdTo);

            return dtos.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            var dtos = await _bitvavoApi.GetTradesAsync(market, limit, start, end, tradeIdFrom, tradeIdTo);

            return dtos.ConvertToKrieptoBodModel();
        }
    }
}
