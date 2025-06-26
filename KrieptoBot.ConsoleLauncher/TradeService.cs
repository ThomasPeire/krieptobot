using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using KrieptoBot.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace KrieptoBot.ConsoleLauncher;

public class TradeService(
    ILogger<TradeService> logger,
    ITrader trader,
    ITradingContext tradingContext,
    IDateTimeProvider dateTimeProvider)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Running in simulation mode: {Simulation}", tradingContext.IsSimulation);

        await WaitForBeginningOfMinute().WaitAsync(cancellationToken);

        StartTrader(null, null);

        var timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
        timer.AutoReset = true;
        timer.Elapsed += StartTrader;
        timer.Start();
    }

    private async Task WaitForBeginningOfMinute()
    {
        var exchangeTime = await dateTimeProvider.UtcDateTimeNowExchange();
        logger.LogInformation("Trader will start on +/- second 5 of first minute of interval (Exchange time)");
        logger.LogInformation("Exchange time: {ExchangeTime}, Local time: {LocalTime}", exchangeTime.ToLocalTime(),
            DateTime.UtcNow.ToLocalTime());

        var secondsUntilNewMinute = 60 - (exchangeTime.Second == 0 ? 60 : exchangeTime.Second);

        await Task.Delay((secondsUntilNewMinute + 5) * 1000);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
        return Task.CompletedTask;
    }

    private async void StartTrader(object sender, ElapsedEventArgs e)
    {
        try
        {
            GroupingGuidEnricher.CurrentGroupingGuid = Guid.NewGuid();

            if (await TraderCanRun())
                await RunTrader();
        }
        catch (Exception exception)
        {
            logger.LogCritical("Error occurred: {Message}", exception.Message);
        }
    }

    private async Task<bool> TraderCanRun()
    {
        var dateTimeNow = await dateTimeProvider.UtcDateTimeNowSyncedWithExchange();
        return CurrentMinuteIsFirstOfInterval(dateTimeNow, tradingContext.PollingIntervalInMinutes);
    }

    private bool CurrentMinuteIsFirstOfInterval(DateTime dateTimeNow, int intervalInMinutes)
    {
        var minutesModulo = (dateTimeNow.Day * 24 * 60 + dateTimeNow.Hour * 60 + dateTimeNow.Minute) %
                            intervalInMinutes;
        var minutesTillRun = intervalInMinutes - minutesModulo;

        if (minutesTillRun != intervalInMinutes)
        {
            logger.LogInformation("Trader will run in {Minute} minutes", minutesTillRun);
        }

        return minutesModulo == 0;
    }

    private async Task RunTrader()
    {
        logger.LogDebug("Starting trading service");
        await tradingContext.SetCurrentTime();
        await trader.Run();
    }
}