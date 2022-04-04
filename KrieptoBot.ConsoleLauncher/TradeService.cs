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
        private readonly IDateTimeProvider _dateTimeProvider;

        public TradeService(ILogger<TradeService> logger, ITrader trader, ITradingContext tradingContext,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _trader = trader;
            _tradingContext = tradingContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            WaitForSecond45();

            if (TraderCanRun())
                await RunTrader();

            var timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += StartTrader;
            timer.Start();
        }

        private void WaitForSecond45()
        {
            _logger.LogInformation("Trader will start on +/- second 45 of last minute of interval");

            while (true)
            {
                if (_dateTimeProvider.UtcDateTimeNow().Second > 45)
                {
                    break;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }

        private async void StartTrader(object sender, ElapsedEventArgs e)
        {
            if (TraderCanRun())
                await RunTrader();
        }

        private bool TraderCanRun()
        {
            var dateTimeNow = _dateTimeProvider.UtcDateTimeNow();
            var intervalInMinutes = GetIntervalInMinutes(_tradingContext.Interval);
            return CurrentMinuteIsLastOfInterval(dateTimeNow, intervalInMinutes);
        }

        private bool CurrentMinuteIsLastOfInterval(DateTime dateTimeNow, int intervalInMinutes)
        {
            var minutesModulo = (dateTimeNow.Day * 24 * 60 + dateTimeNow.Hour * 60 + dateTimeNow.Minute) %
                                intervalInMinutes;
            var minutesTillRun = intervalInMinutes - minutesModulo - 1;

            if (minutesTillRun != 0)
            {
                _logger.LogInformation("Trader will run in {Minute} minutes", minutesTillRun);
            }

            return minutesModulo == intervalInMinutes - 1;
        }

        private async Task RunTrader()
        {
            _logger.LogDebug("Starting trading service");
            _tradingContext.SetCurrentTime();
            await _trader.Run();
        }

        private static int GetIntervalInMinutes(string interval)
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
                _ => 240
            };
        }
    }
}
