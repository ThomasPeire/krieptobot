using System;
using System.Collections.Generic;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public interface IMacd
    {
        Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles);
    }
}
