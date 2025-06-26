using System.Collections.Generic;
using KrieptoBot.Application.Indicators.Results;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators;

public interface IMacd
{
    MacdResult Calculate(IEnumerable<Candle> candles);
}