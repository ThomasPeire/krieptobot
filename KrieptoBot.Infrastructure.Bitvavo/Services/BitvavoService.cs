﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Domain;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Infrastructure.Bitvavo.Services
{
    public class BitvavoService : IExchangeService
    {
        private readonly IBitvavoApi _bitvavoApi;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BitvavoService> _logger;

        public BitvavoService(IBitvavoApi bitvavoApi, IMemoryCache cache, ILogger<BitvavoService> logger)
        {
            _bitvavoApi = bitvavoApi;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Asset> GetAssetAsync(string symbol)
        {
            return await _cache.GetOrCreateAsync($"{nameof(GetAssetAsync)}-{symbol}", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var dto = await _bitvavoApi.GetAssetAsync(symbol);

                return dto.ConvertToKrieptoBotModel();
            });
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            return await _cache.GetOrCreateAsync($"{nameof(GetAssetsAsync)}", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var dtos = await _bitvavoApi.GetAssetsAsync();

                return dtos.ConvertToKrieptoBotModel();
            });
        }

        public async Task<IEnumerable<Balance>> GetBalanceAsync()
        {
            var dtos = await _bitvavoApi.GetBalanceAsync();

            return dtos.ConvertToKrieptoBotModel();
        }

        public async Task<Balance> GetBalanceAsync(string symbol)
        {
            var dto = await _bitvavoApi.GetBalanceAsync(symbol);
            var balance = dto.FirstOrDefault() ??
                          new BalanceDto
                          {
                              Available = "0",
                              Symbol = symbol,
                              InOrder = "0"
                          };

            return balance.ConvertToKrieptoBotModel();
        }

        public async Task<IEnumerable<Candle>> GetCandlesAsync(string market, string interval = Interval.Minutes.Five, int limit = 1000,
            DateTime? start = null, DateTime? end = null, CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync($"{nameof(GetCandlesAsync)}-{market}-{interval}-{limit}-{start}-{end}",
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(Interval.Of(interval).InMinutes() - 1);
                    var candleJArrayList = await _bitvavoApi.GetCandlesAsync(market, interval, limit,
                        start.ToUnixTimeMilliseconds(), end.ToUnixTimeMilliseconds());

                    return candleJArrayList?.Select(x =>
                        new CandleDto
                        {
                            TimeStamp = x.Value<long>(0),
                            Open = decimal.Parse(x.Value<string>(1), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                            High = decimal.Parse(x.Value<string>(2), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                            Low = decimal.Parse(x.Value<string>(3), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                            Close = decimal.Parse(x.Value<string>(4), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                            Volume = decimal.Parse(x.Value<string>(5), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                        }).ConvertToKrieptoBotModel();
                });
        }

        public async Task<Market> GetMarketAsync(string market)
        {
            return await _cache.GetOrCreateAsync($"{nameof(GetMarketAsync)}-{market}", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var dto = await _bitvavoApi.GetMarketAsync(market);

                return dto.ConvertToKrieptoBotModel();
            });
        }

        public async Task<IEnumerable<Market>> GetMarketsAsync()
        {
            return await _cache.GetOrCreateAsync($"{nameof(GetMarketsAsync)}", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                var dtos = await _bitvavoApi.GetMarketsAsync();

                return dtos.ConvertToKrieptoBotModel();
            });
        }

        public async Task<IEnumerable<Order>> GetOpenOrderAsync()
        {
            var dto = await _bitvavoApi.GetOpenOrdersAsync();

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<IEnumerable<Order>> GetOpenOrderAsync(string market)
        {
            var dto = await _bitvavoApi.GetOpenOrdersAsync(market);

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<Order> PostSellOrderAsync(string market, string orderType, decimal amount, decimal price)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                {
                    "market", market
                },
                {
                    "orderType", orderType
                },
                {
                    "side", "sell"
                },
                {
                    "amount", amount.RoundToSignificantDigits(5, MidpointRounding.ToZero)
                        .ToString(CultureInfo.InvariantCulture)
                }
            };

            if (price != 0m)
            {
                paramDictionary.Add("price", price.RoundToSignificantDigits(5, MidpointRounding.ToZero)
                    .ToString(CultureInfo.InvariantCulture));
            }

            _logger.LogInformation(
                "Posting sell order: Market: {Market} - OrderType: {OrderType} - Side: {Side} - Amount: {Amount} - Price: {Price}",
                paramDictionary["market"],
                paramDictionary["orderType"],
                paramDictionary["side"],
                paramDictionary["amount"],
                paramDictionary["price"]);

            var dto = await _bitvavoApi.PostOrderAsync(
                paramDictionary
            );

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<Order> PostStopLossOrderAsync(string market, decimal amount, decimal price)
        {
            var dto = await _bitvavoApi.PostOrderAsync(
                new Dictionary<string, string>
                {
                    {
                        "market", market
                    },
                    {
                        "orderType", OrderType.StopLoss
                    },
                    {
                        "side", "sell"
                    },
                    {
                        "amount", amount.RoundToSignificantDigits(5, MidpointRounding.ToZero)
                            .ToString(CultureInfo.InvariantCulture)
                    },
                    {
                        "triggerAmount", price.RoundToSignificantDigits(5, MidpointRounding.ToZero)
                            .ToString(CultureInfo.InvariantCulture)
                    },
                    {
                        "triggerType", "price"
                    },
                    {
                        "triggerReference", "lastTrade"
                    }
                }
            );

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<Order> PostBuyOrderAsync(string market, string orderType, decimal amount, decimal price)
        {
            var dto = await _bitvavoApi.PostOrderAsync(
                new Dictionary<string, string>
                {
                    {
                        "market", market
                    },
                    {
                        "orderType", orderType
                    },
                    {
                        "side", "buy"
                    },
                    {
                        "amount", amount.RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture)
                    },
                    {
                        "price", price.RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture)
                    }
                }
            );

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<TickerPrice> GetTickerPrice(string market)
        {
            var dto = await _bitvavoApi.GetTickerPrice(market);

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task CancelOrders(string market = "")
        {
            if (string.IsNullOrEmpty(market))
            {
                await _bitvavoApi.CancelOrders();
            }
            else
            {
                await _bitvavoApi.CancelOrders(market);
            }
        }

        public async Task CancelOrder(string market, Guid orderId)
        {
            await _bitvavoApi.CancelOrder(market, orderId);
        }

        public async Task<DateTime> GetTime()
        {
            var time = await _bitvavoApi.GetTime();

            return DateTime.UnixEpoch.AddMilliseconds(time.TimeInMilliseconds);
        }

        public async Task UpdateOrderPrice(string market, Guid id, decimal price)
        {
            await _bitvavoApi.UpdateOrderAsync(
                new Dictionary<string, string>
                {
                    {
                        "market", market
                    },
                    {
                        "orderId", id.ToString()
                    },
                    {
                        "price", price.RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture)
                    }
                }
            );
        }

        public async Task UpdateOrderTriggerAmount(string market, Guid id, decimal price)
        {
            await _bitvavoApi.UpdateOrderAsync(
                new Dictionary<string, string>
                {
                    {
                        "market", market
                    },
                    {
                        "orderId", id.ToString()
                    },
                    {
                        "triggerAmount", price.RoundToSignificantDigits(5).ToString(CultureInfo.InvariantCulture)
                    }
                }
            );
        }

        public async Task<Order> GetOrderAsync(string market, Guid orderId)
        {
            var dto = await _bitvavoApi.GetOrderAsync(market, orderId);

            return dto.ConvertToKrieptoBotModel();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string market, int limit = 500, DateTime? start = null,
            DateTime? end = null, Guid? orderIdFrom = null, Guid? orderIdTo = null)
        {
            var dtos = await _bitvavoApi.GetOrdersAsync(market, limit, start.ToUnixTimeMilliseconds(),
                end.ToUnixTimeMilliseconds(), orderIdFrom, orderIdTo);

            return dtos.ConvertToKrieptoBotModel();
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string market, int limit = 500, DateTime? start = null,
            DateTime? end = null, Guid? tradeIdFrom = null, Guid? tradeIdTo = null)
        {
            var dtos = await _bitvavoApi.GetTradesAsync(market, limit, start.ToUnixTimeMilliseconds(),
                end.ToUnixTimeMilliseconds(), tradeIdFrom, tradeIdTo);

            return dtos.ConvertToKrieptoBotModel();
        }
    }
}
