using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Application.Indicators.Results;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators;

public class Macd : IMacd
{
    private readonly IExponentialMovingAverage _ema;

    public Macd(IExponentialMovingAverage ema)
    {
        _ema = ema;
    }

    public MacdResult Calculate(IEnumerable<Candle> candles)
    {
        var priceDictionary = candles.ToDictionary(x => x.TimeStamp, x => x.Close.Value);
        var ema12 = _ema.Calculate(priceDictionary, 12);
        var ema26 = _ema.Calculate(priceDictionary, 26);
        var datetimeIntersect = ema12.Keys.Intersect(ema26.Keys);
        var macd =
            datetimeIntersect.ToDictionary(datetime => datetime, datetime => ema12[datetime] - ema26[datetime]);

        var signalLine = _ema.Calculate(macd, 9);

        var histogram = signalLine.ToDictionary(x => x.Key, x => macd[x.Key] - x.Value);

        return new MacdResult { MacdLine = macd, SignalLine = signalLine, Histogram = histogram };
    }
}