using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
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
            await _collector.CollectCandles(new List<string> {"BTC-EUR"}, new List<string> {"1h"},
                new DateTime(2020, 01, 01), new DateTime(2021, 01, 01));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}