using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KrieptoBot.Application.Settings
{
    [ExcludeFromCodeCoverage]
    public class TradingSettings
    {
        public string Interval { get; set; }
        public IEnumerable<string> MarketsToWatch { get; set; }
        public DateTime CurrentTime { get; set; }
        public decimal BuyMargin { get; set; }
        public decimal SellMargin { get; set; }
        public bool IsSimulation { get; set; } = true;
        public decimal MaxBuyBudgetPerCoin { get; set; } = 50m;
        public decimal MinBuyBudgetPerCoin { get; set; } = 20m;
        public int PollingIntervalInMinutes { get; set; } = 5;
        public int BuyCoolDownPeriodInMinutes { get; set; } = 5;
    }
}
