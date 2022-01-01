using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.DataCollector
{
    public class CollectorService : IHostedService
    {
        private readonly ICollector _collector;

        public CollectorService(ICollector collector)
        {
            _collector = collector;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _collector.CollectCandles(new List<string> { "BTC-EUR" }, new List<string> { "5m" },
                new DateTime(2021, 11, 01), new DateTime(2021, 11, 30));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}
