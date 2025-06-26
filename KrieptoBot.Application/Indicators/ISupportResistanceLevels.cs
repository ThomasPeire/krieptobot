using System.Collections.Generic;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators;

public interface ISupportResistanceLevels
{
    IEnumerable<SupportResistanceLevel> Calculate(IEnumerable<Candle> candles);
}