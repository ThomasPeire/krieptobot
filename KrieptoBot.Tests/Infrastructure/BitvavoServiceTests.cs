using System;
using System.Threading.Tasks;
using FluentAssertions;
using KrieptoBot.Infrastructure.Bitvavo.Services;
using KrieptoBot.Tests.Mocks.Bitvavo;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace KrieptoBot.Tests.Infrastructure
{
    public class BitvavoServiceTests
    {
        private readonly MockBitvavoApi _mockService = new();
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
            var result = await _bitvavoService.GetCandlesAsync("BTC-EUR");

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
            var result =
                await _bitvavoService.GetOrderAsync("BTC-EUR", new Guid("4a7bd126-2d21-4918-96dc-0c8f51760a0b"));

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOpenOrder_ShouldReturn_OpenOrder()
        {
            var result = await _bitvavoService.GetOpenOrderAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task PostOrder_ShouldReturn_AddOrder()
        {
            var order = await _bitvavoService.PostBuyOrderAsync("BTC-EUR", "Limit", 9000.00m, 0.0001m);

            var result = await _bitvavoService.GetOrderAsync(order.MarketName.Value, order.Id);

            Assert.That(order.MarketName.Value, Is.EqualTo(result.MarketName.Value));
            Assert.That(order.Side, Is.EqualTo(result.Side));
            Assert.That(order.Type, Is.EqualTo(result.Type));
            Assert.That(order.Amount, Is.EqualTo(result.Amount));
            Assert.That(order.Price, Is.EqualTo(result.Price));
        }
    }
}