using System;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.CronJob;

public class TradeService(
    ILogger<TradeService> logger,
    ITrader trader,
    ITradingContext tradingContext)
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Running in simulation mode: {Simulation}", tradingContext.IsSimulation);

        await StartTrader(cancellationToken);
    }

    private async Task StartTrader(CancellationToken cancellationToken)
    {
        try
        {
            GroupingGuidEnricher.CurrentGroupingGuid = Guid.NewGuid();
                
            await RunTrader(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogCritical("Error occurred: {Message}", exception.Message);
        }
    }

    private async Task RunTrader(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting trading service");
        await tradingContext.SetCurrentTime();
        await trader.Run(cancellationToken);
    }
}