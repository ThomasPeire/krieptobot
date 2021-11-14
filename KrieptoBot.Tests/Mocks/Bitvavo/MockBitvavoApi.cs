using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Infrastructure.Bitvavo;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrieptoBot.Tests.Mocks.Bitvavo
{
    public class MockBitvavoApi : IBitvavoApi
    {
        private IEnumerable<AssetDto> _assets;
        private IEnumerable<BalanceDto> _balances;
        private IEnumerable<JArray> _candles;
        private IEnumerable<MarketDto> _markets;
        private ICollection<OrderDto> _orders;
        private IEnumerable<TradeDto> _trades;

        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync()
        {
            return await Task.FromResult(_balances);
        }

        public async Task<IEnumerable<JArray>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000,
            long? start = null,
            long? end = null)
        {
            return await Task.FromResult(_candles);
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

        public async Task<IEnumerable<TradeDto>> GetTradesAsync(string market, int limit = 500, long? start = null,
            long? end = null,
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

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(string market, int limit = 500, long? start = null,
            long? end = null,
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

        public async Task<OrderDto> GetOpenOrderAsync(string market)
        {
            return await Task.FromResult(_orders.First(x => x.Market == market));
        }

        public async Task<OrderDto> PostOrderAsync(string market, string side, string orderType, string amount,
            string price)
        {
            var orderDto =
                new OrderDto
                {
                    OrderId = Guid.NewGuid().ToString(),
                    Market = market,
                    Side = side,
                    OrderType = orderType,
                    Amount = amount,
                    Price = price,
                    Status = "filled"
                };

            _orders.Add(orderDto);

            return await Task.FromResult(orderDto);
        }

        public Task<TickerPriceDto> GetTickerPrice(string market)
        {
            throw new NotImplementedException();
        }

        public void InitData()
        {
            var assetsJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/assets.json");
            var balancesJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/balances.json");
            var candlesJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var marketsJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/markets.json");
            var ordersJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/orders_btc-eur.json");
            var tradesJson = File.ReadAllText(@"./Mocks/Bitvavo/Data/trades_btc-eur.json");

            _assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(assetsJson);
            _balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(balancesJson);
            _candles = JsonConvert.DeserializeObject<IEnumerable<JArray>>(candlesJson);
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson);
            _orders = JsonConvert.DeserializeObject<ICollection<OrderDto>>(ordersJson);
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson);
        }

        public async Task<OrderDto> GetOpenOrderAsync()
        {
            return await GetOpenOrderAsync("");
        }
    }
}
