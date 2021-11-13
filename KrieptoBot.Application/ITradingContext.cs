using System;
using System.Collections.Generic;

namespace KrieptoBot.Application
{
    public interface ITradingContext
    {
        string Interval { get; }
        IEnumerable<string> MarketsToWatch { get; }
        DateTime CurrentTime { get; set; }
        int BuyMargin { get; }
        int SellMargin { get; }
        bool IsSimulation { get; }
    }
}