using KrieptoBod.Infrastructure.Exchange;
using KrieptoBod.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Tests.Infrastructure
{
    public class Tests
    {
        private readonly Mock<IExchangeService> _exchangeService = new Mock<IExchangeService>();

        [SetUp]
        public void Setup()
        {
            //Todo use mock data
            _exchangeService
                .Setup(service => service.GetBalanceAsync())
                .Returns(Task.FromResult(new List<Balance>().AsEnumerable()));
            _exchangeService
                .Setup(service => service.GetCandlesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Candle>().AsEnumerable()));

            _exchangeService
                .Setup(service => service.GetMarketsAsync())
                .Returns(Task.FromResult(new List<Market>().AsEnumerable()));

            _exchangeService
                .Setup(service => service.GetMarketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Market()));

            _exchangeService
                .Setup(service => service.GetAssetsAsync())
                .Returns(Task.FromResult(new List<Asset>().AsEnumerable()));

            _exchangeService
                .Setup(service => service.GetAssetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Asset()));

            _exchangeService
                .Setup(service => service.GetTradesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<Trade>().AsEnumerable()));

            _exchangeService
                .Setup(service => service.GetOrderAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Order()));

            _exchangeService
                .Setup(service => service.GetOrdersAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<Order>().AsEnumerable()));

            _exchangeService
                .Setup(service => service.GetOpenOrderAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Order()));

        }

        [Test]
        public async Task GetBalance_ShouldReturn_Balance()
        {
            var exchangeRepository = new ExchangeRepository(_exchangeService.Object);

            var result = await exchangeRepository.GetBalanceAsync();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetCandles_ShouldReturn_Candles()
        {
            var exchangeRepository = new ExchangeRepository(_exchangeService.Object);

            var result = await exchangeRepository.GetCandlesAsync("", "", 0, DateTime.Now, DateTime.Now);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetMarkets_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetMarketsAsync(), Throws.Exception);
        }

        [Test]
        public void GetMarket_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetMarketAsync(""), Throws.Exception);
        }

        [Test]
        public void GetAssets_ShouldThrow_NotImplementedException()
        {
            Assert.That(async() => await new ExchangeRepository(_exchangeService.Object).GetAssetsAsync(), Throws.Exception);
        }

        [Test]
        public void GetAsset_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetAssetAsync(""), Throws.Exception);
        }

        [Test]
        public void GetTrades_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetTradesAsync(""), Throws.Exception);
        }

        [Test]
        public void GetOrders_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetOrdersAsync(""), Throws.Exception);
        }

        [Test]
        public void GetOrder_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetOrderAsync("", new Guid()), Throws.Exception);
        }

        [Test]
        public void GetOpenOrder_ShouldThrow_NotImplementedException()
        {
            Assert.That(async () => await new ExchangeRepository(_exchangeService.Object).GetOpenOrderAsync(""), Throws.Exception);
        }
    }
}