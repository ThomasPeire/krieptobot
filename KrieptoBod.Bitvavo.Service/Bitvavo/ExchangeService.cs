using KrieptoBod.Exchange.Bitvavo.Helpers;
using KrieptoBod.Exchange.Bitvavo.Model;
using KrieptoBod.Infrastructure.Exchange;
using KrieptoBod.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace KrieptoBod.Exchange.Bitvavo
{
    public class ExchangeService : IExchangeService
    {
        private readonly BitvavoClient _client;

        public ExchangeService(BitvavoClient client)
        {
            _client = client;
        }

        public async Task<Asset> GetAssetAsync(string symbol)
        {
            var queryString =
                new QueryString()
                    .Add("symbol", symbol);

            var assetDto = await Deserialize<AssetDto>(await _client.GetAsync($"/v2/assets{queryString.ToUriComponent()}"));

            return assetDto.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            var dtoEnumerable = await Deserialize<IEnumerable<AssetDto>>(await _client.GetAsync($"/v2/assets"));

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var dtoEnumerable = await Deserialize<IEnumerable<BalanceDto>>(await _client.GetAsync("/v2/balance"));

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }
        public async Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = "5m", int limit = 1000, DateTime? start = null, DateTime? end = null)
        {
            var queryString =
                new QueryString()
                    .Add("interval", interval)
                    .Add("limit", limit.ToString());

            if (start != null)
            {
                queryString.Add("start", ((DateTimeOffset)start).ToUnixTimeSeconds().ToString());
            }

            if (end != null)
            {
                queryString.Add("end", ((DateTimeOffset)end).ToUnixTimeSeconds().ToString());
            }

            var response = await _client.GetAsync($"/v2/{market}/candles{queryString.ToUriComponent()}");
            var responseContent = await response.ReadAsStringAsync();
            var deserializedCandles = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent) as Newtonsoft.Json.Linq.JArray;
            var dtoEnumerable = deserializedCandles?.Select(x =>
                new CandleDto()
                {
                    TimeStamp = DateTime.UnixEpoch.AddMilliseconds(x.Value<long>(0)),
                    Open = x.Value<decimal>(1),
                    High = x.Value<decimal>(2),
                    Low = x.Value<decimal>(3),
                    Close = x.Value<decimal>(4),
                    Volume = x.Value<decimal>(5),
                });

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }

        public async Task<Market> GetMarketAsync(string market)
        {
            var queryString =
                new QueryString()
                    .Add("market", market);

            var dto = await Deserialize<MarketDto>(await _client.GetAsync($"/v2/markets{queryString.ToUriComponent()}"));

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            var dtoEnumerable = await Deserialize<IEnumerable<MarketDto>>(await _client.GetAsync($"/v2/markets"));

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            var queryString =
                new QueryString()
                    .Add("limit", limit.ToString());

            if (start != null)
            {
                queryString.Add("start", ((DateTimeOffset)start).ToUnixTimeSeconds().ToString());
            }

            if (end != null)
            {
                queryString.Add("end", ((DateTimeOffset)end).ToUnixTimeSeconds().ToString());
            }

            if (tradeIdFrom != null)
            {
                queryString.Add("tradeIdFrom", tradeIdFrom.ToString());
            }

            if (tradeIdTo != null)
            {
                queryString.Add("tradeIdFrom", tradeIdTo.ToString());
            }

            var dtoEnumerable = await Deserialize<IEnumerable<TradeDto>>(await _client.GetAsync($"/v2/{market}/trades{queryString.ToUriComponent()}"));

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null, DateTime? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            var queryString =
                new QueryString()
                    .Add("market", market)
                    .Add("limit", limit.ToString());

            if (start != null)
            {
                queryString.Add("start", ((DateTimeOffset)start).ToUnixTimeSeconds().ToString());
            }
            
            if (end != null)
            {
                queryString.Add("end", ((DateTimeOffset)end).ToUnixTimeSeconds().ToString());
            }
            
            if (orderIdFrom != null)
            {
                queryString.Add("orderIdFrom", orderIdFrom.ToString());
            }
            
            if (orderIdTo != null)
            {
                queryString.Add("orderIdTo", orderIdTo.ToString());
            }

            var dtoEnumerable = await Deserialize<IEnumerable<OrderDto>>(await _client.GetAsync($"/v2/orders{queryString.ToUriComponent()}"));

            return dtoEnumerable.ConvertToKrieptoBodModel();
        }

        public async Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            var queryString =
                new QueryString()
                    .Add("market", market)
                    .Add("orderId", orderId.ToString());

            var dto = await Deserialize<OrderDto>(await _client.GetAsync($"/v2/order{queryString.ToUriComponent()}"));

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<Order> GetOpenOrderAsync()
        {
            return await GetOpenOrderAsync("");
        }

        public async Task<Order> GetOpenOrderAsync(string market)
        {
            var queryString = new QueryString();

            if (!string.IsNullOrWhiteSpace(market))
            {
                queryString.Add("market", market);
            }

            var dto = await Deserialize<OrderDto>(await _client.GetAsync($"/v2/ordersOpen{queryString.ToUriComponent()}"));

            return dto.ConvertToKrieptoBodModel();
        }

        public async Task<T> Deserialize<T>(HttpContent content)
        {
            return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
    }
}
