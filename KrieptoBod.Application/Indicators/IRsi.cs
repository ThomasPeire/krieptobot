using KrieptoBod.Model;
using System;
using System.Collections.Generic;

namespace KrieptoBod.Application.Indicators
{
    public interface IRsi
    {
        Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int avgPeriod);
    }
}
