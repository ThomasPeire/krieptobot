using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.ConsoleLauncher
{
    public class TradeService : IHostedService
    {
        private readonly ITrader _trader;

        public TradeService(ITrader trader)
        {
            _trader = trader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _trader.Run();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}