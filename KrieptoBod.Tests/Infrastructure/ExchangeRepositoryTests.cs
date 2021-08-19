using KrieptoBod.Infrastructure.Exchange;
using KrieptoBod.Tests.Mocks.Bitvavo;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Snapshooter.NUnit;

namespace KrieptoBod.Tests.Infrastructure
{
    public class ExchangeRepositoryTests
    {
        private readonly MockService _mockService = new MockService();
        private ExchangeRepository _exchangeRepository;

        [SetUp]
        public void Setup()
        {
            _mockService.InitData();
            _exchangeRepository = new ExchangeRepository(_mockService);
        }

        [Test]
        public async Task GetBalance_ShouldReturn_Balance()
        {
            var result = await _exchangeRepository.GetBalanceAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetCandles_ShouldReturn_Candles()
        {
            var result = await _exchangeRepository.GetCandlesAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetMarkets_ShouldReturn_Markets()
        {
            var result = await _exchangeRepository.GetMarketsAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetMarket_ShouldReturn_Market()
        {
            var result = await _exchangeRepository.GetMarketAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetAssets_ShouldReturn_Assets()
        {
            var result = await _exchangeRepository.GetAssetsAsync();

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetAsset_ShouldReturn_Asset()
        {
            var result = await _exchangeRepository.GetAssetAsync("BTC");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetTrades_ShouldReturn_Trades()
        {
            var result = await _exchangeRepository.GetTradesAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOrders_ShouldReturn_Orders()
        {
            var result = await _exchangeRepository.GetOrdersAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOrder_ShouldReturn_Order()
        {
            var result = await _exchangeRepository.GetOrderAsync("BTC-EUR", new Guid("4a7bd126-2d21-4918-96dc-0c8f51760a0b"));

            result.Should().MatchSnapshot();
        }

        [Test]
        public async Task GetOpenOrder_ShouldReturn_OpenOrder()
        {
            var result = await _exchangeRepository.GetOpenOrderAsync("BTC-EUR");

            result.Should().MatchSnapshot();
        }
    }
}