using FluentAssertions;
using KrieptoBod.Exchange.Bitvavo.Helpers;
using KrieptoBod.Exchange.Bitvavo.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using Snapshooter.NUnit;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBod.Tests.Exchange.Bitvavo.Helpers
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
            var assetsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/assets.json");
            var balancesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/balances.json");
            var candlesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var marketsJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/markets.json");
            var ordersJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/orders_btc-eur.json");
            var tradesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/trades_btc-eur.json");

            _assets = JsonConvert.DeserializeObject<IEnumerable<AssetDto>>(assetsJson);
            _balances = JsonConvert.DeserializeObject<IEnumerable<BalanceDto>>(balancesJson);
            //_candles = JsonConvert.DeserializeObject<IEnumerable<CandleDto>>(candlesJson);
            _markets = JsonConvert.DeserializeObject<IEnumerable<MarketDto>>(marketsJson);
            _orders = JsonConvert.DeserializeObject<IEnumerable<OrderDto>>(ordersJson);
            _trades = JsonConvert.DeserializeObject<IEnumerable<TradeDto>>(tradesJson);
        }

        [Test]
        public void ConvertToKrieptoBodModel_ShouldConvert_BitvavoAssetToKrieptoBodAsset()
        {
            var result = _assets.First().ConvertToKrieptoBodModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBodModel_ShouldConvert_BitvavoBalanceToKrieptoBodBalance()
        {
            var result = _balances.First().ConvertToKrieptoBodModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBodModel_ShouldConvert_BitvavoMarketToKrieptoBodMarket()
        {
            var result = _markets.First().ConvertToKrieptoBodModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBodModel_ShouldConvert_BitvavoOrderToKrieptoBodOrder()
        {
            var result = _orders.First().ConvertToKrieptoBodModel();

            result.Should().MatchSnapshot();
        }

        [Test]
        public void ConvertToKrieptoBodModel_ShouldConvert_BitvavoTradeToKrieptoBodTrade()
        {
            var result = _trades.First().ConvertToKrieptoBodModel();

            result.Should().MatchSnapshot();
        }

    }
}