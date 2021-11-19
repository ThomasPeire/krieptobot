using System;
using System.Collections.Generic;

namespace KrieptoBot.Application.Settings
{
    public class TradingSettings
    {
        public string Interval { get; set; }
        public IEnumerable<string> MarketsToWatch { get; set; }
        public DateTime CurrentTime { get; set; }
        public int BuyMargin { get; set; }
        public int SellMargin { get; set; }
        public bool IsSimulation { get; set; }
    }
}
