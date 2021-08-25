using KrieptoBot.Model;
using System;
using System.Collections.Generic;

namespace KrieptoBot.Application.Indicators
{
    public interface IRsi
    {
        Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int avgPeriod);
    }
}
