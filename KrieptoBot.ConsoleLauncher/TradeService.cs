using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using KrieptoBot.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace KrieptoBot.ConsoleLauncher
{
    public class TradeService : IHostedService
    {
        private readonly ILogger<TradeService> _logger;
        private readonly ITrader _trader;
        private readonly ITradingContext _tradingContext;

        public TradeService(ILogger<TradeService> logger, ITrader trader, ITradingContext tradingContext)
        {
            _logger = logger;
            _trader = trader;
            _tradingContext = tradingContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RunTrader();

            var timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += StartTrader;
            timer.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }

        private async void StartTrader(object sender, ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Minute % GetIntervalInMinutes(_tradingContext.Interval) != 0)
                return;
            await RunTrader();
        }

        private async Task RunTrader()
        {
            _logger.LogDebug("Starting trading service");
            _tradingContext.CurrentTime = DateTime.UtcNow;
            await _trader.Run();
        }

        private int GetIntervalInMinutes(string interval)
        {
            return interval switch
            {
                "1m" => 1,
                "5m" => 5,
                "15m" => 15,
                "30m" => 30,
                "1h" => 60,
                "2h" => 120,
                "4h" => 240,
                "6h" => 360,
                "8h" => 480,
                "12h" => 720,
                "1d" => 1440,
                _ => 0
            };
        }
    }
}