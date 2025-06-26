using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.DataCollector;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KrieptoBot.Tests.DataCollector;

public class CollectorServiceTests
{
    private Mock<ICollector> _mockCollector = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CollectorServiceService_Should_RunCollector()
    {
        var collectorService = new CollectorService(_mockCollector.Object);

        await collectorService.StartAsync(new CancellationToken());
        _mockCollector.Verify(
            x => x.CollectCandles(It.IsAny<IEnumerable<string>>(), It.IsAny<ICollection<string>>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void TradeServiceStopAsync_Should_ReturnTaskComplete()
    {
        var collectorService = new CollectorService(_mockCollector.Object);

        var result = collectorService.StopAsync(new CancellationToken());

        Assert.That(result, Is.EqualTo(Task.CompletedTask));
    }
}