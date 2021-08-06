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
        }

        [Test]
        public async Task GetBalance_ShouldReturn_Balance()
        {
            var exchangeRepository = new ExchangeRepository(_exchangeService.Object);

            var result = await exchangeRepository.GetBalanceAsync();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetCandles_ShouldReturn_Balance()
        {
            var exchangeRepository = new ExchangeRepository(_exchangeService.Object);

            var result = await exchangeRepository.GetCandlesAsync("", "", 0, DateTime.Now, DateTime.Now);

            Assert.That(result, Is.Not.Null);
        }
    }
}