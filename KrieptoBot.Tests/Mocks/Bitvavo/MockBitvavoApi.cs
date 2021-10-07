using KrieptoBot.Application;
using KrieptoBot.Infrastructure.Bitvavo;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using KrieptoBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrieptoBot.Tests.Mocks.Bitvavo
{
    public class MockBitvavoApi : IBitvavoApi
    {
        private IEnumerable<AssetDto> _assets;
        private IEnumerable<BalanceDto> _balances;
        private IEnumerable<JArray> _candles;
        private IEnumerable<MarketDto> _markets;
        private IEnumerable<OrderDto> _orders;
        private IEnumerable<TradeDto> _trades;

        public void InitData()
        {
            var assetsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/assets.json");
            var balancesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/balances.json");
            var candlesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var marketsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/markets.json");
            var ordersJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/orders_btc-eur.json");
            var tradesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/trades_btc-eur.json");

            _assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(assetsJson);
            _balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(balancesJson);
            _candles = JsonConvert.DeserializeObject<IEnumerable<JArray>>(candlesJson);
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson);
            _orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(ordersJson);
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson);
        }

        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync()
        {
            return await Task.FromResult(_balances);
        }

        public async Task<IEnumerable<JArray>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, long? start = null,
            long? end = null)
        {
            return await Task.FromResult(_candles) ;
        }

        public async Task<IEnumerable<MarketDto>> GetMarketsAsync()
        {
            return await Task.FromResult(_markets);
        }

        public async Task<MarketDto> GetMarketAsync(string market)
        {
            return await Task.FromResult(_markets.First(x => x.MarketName == market));
        }

        public async Task<IEnumerable<AssetDto>> GetAssetsAsync()
        {
            return await Task.FromResult(_assets);
        }

        public async Task<AssetDto> GetAssetAsync(string symbol)
        {
            return await Task.FromResult(_assets.First(x => x.Symbol == symbol));
        }

        public async Task<IEnumerable<TradeDto>> GetTradesAsync(string market, int limit = 500, long? start = null, long? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            return await Task.FromResult(
                _trades
                    .Where(x =>
                        x.Timestamp >= start &&
                        x.Timestamp < end &&
                        string.CompareOrdinal(x.Id, tradeIdFrom.ToString()) >= 0 &&
                        string.CompareOrdinal(x.Id, tradeIdFrom.ToString()) < 0)
                    .Take(limit));
        }

        public async Task<OrderDto> GetOrderAsync(string market, Guid orderId)
        {
            return await Task.FromResult(
                _orders.First(x =>
                    x.Market == market &&
                    x.OrderId == orderId.ToString()));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(string market, int limit = 500, long? start = null, long? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            return await Task.FromResult(
                _orders
                    .Where(x =>
                        x.Market == market &&
                        x.Created >= start &&
                        x.Created < end &&
                        string.CompareOrdinal(x.OrderId, orderIdFrom.ToString()) >= 0 &&
                        string.CompareOrdinal(x.OrderId, orderIdTo.ToString()) < 0)
                    .Take(limit));
        }

        public async Task<OrderDto> GetOpenOrderAsync()
        {
            return await GetOpenOrderAsync("");
        }

        public async Task<OrderDto> GetOpenOrderAsync(string market)
        {
            return await Task.FromResult(_orders.First(x => x.Market == market));
        }
    }
}
