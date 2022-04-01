using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public class MovingAverage : IMovingAverage
    {
        public Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int averagePeriod)
        {
            var pricesToAverage = candles.Select(x => new KeyValuePair<DateTime, decimal>(x.TimeStamp, x.Close.Value))
                .ToDictionary(key => key.Key, value => value.Value);

            return CalculateMovingAverage(pricesToAverage, averagePeriod);
        }

        private static Dictionary<DateTime, decimal> CalculateMovingAverage(
            Dictionary<DateTime, decimal> prices, int movingAveragePeriod)
        {
            var movingAverages = new Dictionary<DateTime, decimal>();
            var priceArray = prices.OrderBy(x => x.Key).ToArray();

            for (var i = 0; i < priceArray.Length; i++)
            {
                var average = priceArray.Skip(Math.Max(0, i - movingAveragePeriod + 1)).Take(movingAveragePeriod)
                    .Average(x => x.Value);

                movingAverages.Add(priceArray[i].Key, average);
            }

            return movingAverages;
        }
    }
}
