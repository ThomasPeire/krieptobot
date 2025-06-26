using System;
using System.Collections.Generic;
using KrieptoBot.Application.Indicators.Results;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators;

public interface IRsi
{
    RsiResult Calculate(IEnumerable<Candle> candles, int averagePeriod);
    RsiResult CalculateWithEma(IEnumerable<Candle> candles, int averagePeriod, int emaPeriod);
}