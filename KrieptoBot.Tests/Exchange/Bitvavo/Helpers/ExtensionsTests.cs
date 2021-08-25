using FluentAssertions;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Snapshooter.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBot.Tests.Exchange.Bitvavo.Helpers
{
    public class ExtensionsTests
    {
        private IEnumerable<AssetDto> _assets;
        private IEnumerable<BalanceDto> _balances;
        private IEnumerable<CandleDto> _candles;
        private IEnumerable<MarketDto> _markets;
        private IEnumerable<OrderDto> _orders;
        private IEnumerable<TradeDto> _trades;

        [SetUp]
        public void Setup()
        {
            InitAssets();
            InitBalances();
            InitMarkets();
            InitCandles();
            InitOrders();
            InitTrades();
        }

        private void InitAssets()
        {
            var assetsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/assets.json");
            _assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(assetsJson);
        }

        private void InitBalances()
        {
            var balancesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/balances.json");
            _balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(balancesJson);
        }

        private void InitMarkets()
        { 
            var marketsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/markets.json");
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson);
        }

        private void InitCandles()
        {
            var candlesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var deserializedCandles = JsonConvert.DeserializeObject(candlesJson) as JArray;
            _candles = deserializedCandles.Select(x =>
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

        private void InitOrders()
        {
            var ordersJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/orders_btc-eur.json");
            _orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(ordersJson);
        }

        private void InitTrades()
        {
            var tradesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/trades_btc-eur.json");
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson);
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoAssetToKrieptoBotAsset()
        {
            var result = _assets.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoBalanceToKrieptoBotBalance()
        {
            var result = _balances.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoCandleToKrieptoBotCandle()
        {
            var result = _candles.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoMarketToKrieptoBotMarket()
        {
            var result = _markets.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoOrderToKrieptoBotOrder()
        {
            var result = _orders.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBotModel_ShouldConvert_BitvavoTradeToKrieptoBotTrade()
        {
            var result = _trades.First().ConvertToKrieptoBotModel();

            result.Should().MatchSnapshot();
        }

    }
}