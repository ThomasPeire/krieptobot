using System.Collections.Generic;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public interface ISupportResistanceLevels
    {
        IDictionary<Price, int> CalculateLevelsWithStrength(IEnumerable<Candle> candles);
    }
}
