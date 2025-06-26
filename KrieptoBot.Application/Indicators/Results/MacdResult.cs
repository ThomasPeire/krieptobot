using System;
using System.Collections.Generic;

namespace KrieptoBot.Application.Indicators.Results;

public class MacdResult
{
    public Dictionary<DateTime, decimal> MacdLine { get; set; }
    public Dictionary<DateTime, decimal> SignalLine { get; set; }
    public Dictionary<DateTime, decimal> Histogram { get; set; }
}