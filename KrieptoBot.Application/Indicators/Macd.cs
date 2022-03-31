using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public class Macd : IMacd
    {
        private readonly IExponentialMovingAverage _ema;

        public Macd(IExponentialMovingAverage ema)
        {
            _ema = ema;
        }

        public Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles)
        {
            var ema12 = _ema.Calculate(candles, 12);
            var ema26 = _ema.Calculate(candles, 26);
            var datetimeIntersect = ema12.Keys.Intersect(ema26.Keys);
            var difference =
                datetimeIntersect.ToDictionary(datetime => datetime, datetime => ema12[datetime] - ema26[datetime]);
            return difference;
            // todo
            // do not return just yet, first calculate 9 period EMA from 'difference'
            // subtract 9 period EMA from 'difference'
        }
    }
}
