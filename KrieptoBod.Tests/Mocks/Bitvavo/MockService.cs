using KrieptoBod.Exchange.Bitvavo.Helpers;
using KrieptoBod.Exchange.Bitvavo.Model;
using KrieptoBod.Infrastructure.Exchange;
using KrieptoBod.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Tests.Mocks.Bitvavo
{
    public class MockService : IExchangeService
    {
        private IEnumerable<Asset> _assets;
        private IEnumerable<Balance> _balances;
        private IEnumerable<Candle> _candles;
        private IEnumerable<Market> _markets;
        private IEnumerable<Order> _orders;
        private IEnumerable<Trade> _trades;

        public void InitData()
        {
            var assetsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/assets.json");
            var balancesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/balances.json");
            var candlesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var marketsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/markets.json");
            var ordersJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/orders_btc-eur.json");
            var tradesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/trades_btc-eur.json");

            _assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(assetsJson).ConvertToKrieptoBodModel();
            _balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(balancesJson).ConvertToKrieptoBodModel();
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson).ConvertToKrieptoBodModel();
            _orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(ordersJson).ConvertToKrieptoBodModel();
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson).ConvertToKrieptoBodModel();
            _candles = DeserializeCandles(candlesJson).ConvertToKrieptoBodModel();
        }

        private IEnumerable<CandleDto> DeserializeCandles(string candlesJson)
        {
            var deserializedCandles = JsonConvert.DeserializeObject(candlesJson) as Newtonsoft.Json.Linq.JArray;
            return deserializedCandles?.Select(x =>
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

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            return await Task.FromResult(_balances);
        }

        public async Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, DateTime? start = null,
            DateTime? end = null)
        {
            return await Task.FromResult(
                _candles
                    .Where(x =>
                        (x.TimeStamp >= start || start == null) &&
                        (x.TimeStamp < end || end == null))
                    .Take(limit));
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            return await Task.FromResult(_markets);
        }

        public async Task<Market> GetMarketAsync(string market)
        {
            return await Task.FromResult(_markets.First(x => x.MarketName == market));
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            return await Task.FromResult(_assets);
        }

        public async Task<Asset> GetAssetAsync(string symbol)
        {
            return await Task.FromResult(_assets.First(x => x.Symbol == symbol));
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            return await Task.FromResult(
                _trades
                    .Where(x =>
                        (x.Timestamp >= start || start == null) &&
                        (x.Timestamp < end || end == null) &&
                        string.CompareOrdinal(x.Id, tradeIdFrom.ToString()) >= 0 &&
                        string.CompareOrdinal(x.Id, tradeIdFrom.ToString()) < 0)
                    .Take(limit));
        }

        public async Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            return await Task.FromResult(
                _orders.First(x =>
                    x.Market == market &&
                    x.OrderId == orderId.ToString()));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null,
            Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            return await Task.FromResult(
                _orders
                    .Where(x =>
                        x.Market == market &&
                        (x.Created >= start || start == null) &&
                        (x.Created < end || end == null) &&
                        string.CompareOrdinal(x.OrderId, orderIdFrom.ToString()) >= 0 &&
                        string.CompareOrdinal(x.OrderId, orderIdTo.ToString()) < 0)
                    .Take(limit));
        }

        public async Task<Order> GetOpenOrderAsync()
        {
            return await GetOpenOrderAsync("");
        }

        public async Task<Order> GetOpenOrderAsync(string market)
        {
            return await Task.FromResult(_orders.First(x => x.Market == market));
        }
    }
}
