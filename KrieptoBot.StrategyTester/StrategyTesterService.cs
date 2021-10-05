using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.StrategyTester
{
    public class StrategyTesterService : IHostedService
    {

        public StrategyTesterService()
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"starting the service");

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}