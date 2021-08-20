using KrieptoBod.Tests.Mocks.Bitvavo;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Snapshooter.NUnit;
using KrieptoBod.Infrastructure.Bitvavo;
using KrieptoBod.Infrastructure.Bitvavo.Services;

namespace KrieptoBod.Tests.Infrastructure
{
    public class BitvavoServiceTests
    {
        private readonly MockBitvavoApi _mockService = new MockBitvavoApi();
        private BitvavoService _bitvavoService;

        [SetUp]
        public void Setup()
        {
            _mockService.InitData();
            _bitvavoService = new BitvavoService(_mockService);
        }

        [Test]
        public async Task GetBalance_ShouldReturn_Balance()
        {
            var result = await _bitvavoService.GetBalanceAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetCandles_ShouldReturn_Candles()
        {
            var result = await _bitvavoService.GetCandlesAsync("");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetMarkets_ShouldReturn_Markets()
        {
            var result = await _bitvavoService.GetMarketsAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetMarket_ShouldReturn_Market()
        {
            var result = await _bitvavoService.GetMarketAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetAssets_ShouldReturn_Assets()
        {
            var result = await _bitvavoService.GetAssetsAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetAsset_ShouldReturn_Asset()
        {
            var result = await _bitvavoService.GetAssetAsync("BTC");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetTrades_ShouldReturn_Trades()
        {
            var result = await _bitvavoService.GetTradesAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOrders_ShouldReturn_Orders()
        {
            var result = await _bitvavoService.GetOrdersAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOrder_ShouldReturn_Order()
        {
            var result = await _bitvavoService.GetOrderAsync("BTC-EUR", new Guid("4a7bd126-2d21-4918-96dc-0c8f51760a0b"));

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOpenOrder_ShouldReturn_OpenOrder()
        {
            var result = await _bitvavoService.GetOpenOrderAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }
    }
}