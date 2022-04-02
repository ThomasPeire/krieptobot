using System;
using System.Collections.Generic;

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
        TradingContext SetCurrentTime();
    }
}
