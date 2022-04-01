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
            var priceDictionary = candles.ToDictionary(x => x.TimeStamp, x => x.Close.Value);
            var ema12 = _ema.Calculate(priceDictionary, 12);
            var ema26 = _ema.Calculate(priceDictionary, 26);
            var datetimeIntersect = ema12.Keys.Intersect(ema26.Keys);
            var difference =
                datetimeIntersect.ToDictionary(datetime => datetime, datetime => ema12[datetime] - ema26[datetime]);

            var ema9 = _ema.Calculate(difference, 9);

            return ema9.ToDictionary(x => x.Key, x => difference[x.Key] - x.Value);
        }
    }
}
