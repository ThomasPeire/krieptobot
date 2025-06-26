using System;
using System.Collections.Generic;

namespace KrieptoBot.Application.Indicators.Results;

public class RsiResult
{
    public Dictionary<DateTime, decimal> RsiValues { get; set; } = new();
    public Dictionary<DateTime, decimal> EmaValues { get; set; } = new();
}