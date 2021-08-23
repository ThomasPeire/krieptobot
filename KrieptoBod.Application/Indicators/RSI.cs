using KrieptoBod.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBod.Application.Indicators
{
    class Rsi
    {
        private readonly IExchangeService _exchangeService;

        public Rsi(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        public static Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int avgPeriod)
        {
            var upsAndDownMoves = CalculateUpAndDownMoves(candles);

            var avgUpsAndAvgDowns = CalculateSimpleRSI(upsAndDownMoves, avgPeriod);

            var rsiValues = CalculateRSI(avgUpsAndAvgDowns);

            return rsiValues;

        }

        private static Dictionary<DateTime, decimal> CalculateRSI(Dictionary<DateTime, (decimal avgsUp, decimal avgsDown)> avgUpsAndAvgDowns)
        {
            return new(avgUpsAndAvgDowns.Select(x =>
                new KeyValuePair<DateTime, decimal>(x.Key, 100 - (100 / (1 + (x.Value.avgsUp / x.Value.avgsDown))))));
        }

        private static Dictionary<DateTime, (decimal avgsUp, decimal avgsDown)> CalculateSimpleRSI(Dictionary<DateTime, (decimal ups, decimal downs)> upsAndDownMoves, int avgPeriod)
        {
            var movingAverages = new Dictionary<DateTime, (decimal, decimal)>();
            var upsAndDownMovesArray = upsAndDownMoves.OrderBy(x => x.Key).ToArray();

            // start with i=avgPeriod since we need the previous avgPeriod values to calculate
            for (var i = avgPeriod; i < upsAndDownMovesArray.Length; i++)
            {
                var previousValues = upsAndDownMovesArray.Skip(i).Take(-avgPeriod).Select(x => x.Value).ToList();

                var avgUp = previousValues.Average(x => x.ups);
                var avgDown = previousValues.Average(x => x.downs);

                movingAverages.Add(upsAndDownMovesArray[i].Key, (avgUp, avgDown));
            }

            return movingAverages;
        }

        private static Dictionary<DateTime, (decimal ups, decimal downs)> CalculateUpAndDownMoves(IEnumerable<Candle> candles)
        {
            var upsAndDowns = new Dictionary<DateTime, (decimal, decimal)>();

            var candleArray = candles.OrderBy(candle => candle.TimeStamp).ToArray();

            // start with i=1 since we need candle i-1 to calculate
            for (var i = 1; i < candleArray.Length; i++)
            {
                var priceChange = candleArray[i].Close - candleArray[i - 1].Close;

                var up = Math.Max(priceChange, 0);
                var down = Math.Abs(Math.Min(priceChange, 0));
                upsAndDowns.Add(candleArray[i].TimeStamp, (up, down));
            }

            return upsAndDowns;
        }
    }
}
