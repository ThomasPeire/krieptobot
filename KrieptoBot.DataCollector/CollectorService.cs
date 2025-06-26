using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Domain;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.DataCollector;

public class CollectorService(ICollector collector) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return collector.CollectCandles(["BTC-EUR"], [Interval.FiveMinutes],
            DateTime.Today.AddDays(-30), DateTime.Today, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
        return Task.CompletedTask;
    }
}