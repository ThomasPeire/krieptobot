using KrieptoBod.Application;
using KrieptoBod.Infrastructure.Bitvavo;
using KrieptoBod.Infrastructure.Bitvavo.Dtos;
using KrieptoBod.Infrastructure.Bitvavo.Extensions;
using KrieptoBod.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Tests.Mocks.Bitvavo
{
    public class MockBitvavoApi : IBitvavoApi
    {
        private IEnumerable<AssetDto> _assets;
        private IEnumerable<BalanceDto> _balances;
        private IEnumerable<CandleDto> _candles;
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
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson);
            _orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(ordersJson);
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson);
            var deserializedCandles = JsonConvert.DeserializeObject(candlesJson) as Newtonsoft.Json.Linq.JArray;
            _candles = deserializedCandles?.Select(x =>
                new CandleDto
                {
                    TimeStamp = DateTime.UnixEpoch.AddMilliseconds(x.Value<long>(0)),
                    Open = x.Value<decimal>(1),
                    High = x.Value<decimal>(2),
                    Low = x.Value<decimal>(3),
                    Close = x.Value<decimal>(4),
                    Volume = x.Value<decimal>(5),
                });
        }

        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync()
        {
            return await Task.FromResult(_balances);
        }

        public async Task<IEnumerable<CandleDto>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, DateTime? start = null,
            DateTime? end = null)
        {
            return await Task.FromResult(
                _candles
                    .Where(x =>
                        x.TimeStamp >= start &&
                        x.TimeStamp < end)
                    .Take(limit));
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

        public async Task<IEnumerable<TradeDto>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            return await Task.FromResult(
                _trades
                    .Where(x =>
                        DateTime.UnixEpoch.AddMilliseconds(x.Timestamp) >= start &&
                        DateTime.UnixEpoch.AddMilliseconds(x.Timestamp) < end &&
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

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            return await Task.FromResult(
                _orders
                    .Where(x =>
                        x.Market == market &&
                        DateTime.UnixEpoch.AddMilliseconds(x.Created) >= start &&
                        DateTime.UnixEpoch.AddMilliseconds(x.Created) < end &&
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
