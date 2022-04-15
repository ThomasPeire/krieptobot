using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public interface ITradingContext
    {
        string Interval { get; }
        IEnumerable<string> MarketsToWatch { get; }
        DateTime CurrentTime { get; }
        decimal BuyMargin { get; }
        decimal SellMargin { get; }
        bool IsSimulation { get; }
        Task<TradingContext> SetCurrentTime();
        int PollingIntervalInMinutes { get; }
        int BuyCoolDownPeriodInMinutes { get; }
    }
}
