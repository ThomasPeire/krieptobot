using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public class ExponentialMovingAverage : IExponentialMovingAverage
    {
        private const decimal Smoothing = 2m;
        private decimal _multiplier = 1m;

        public Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int averagePeriod)
        {
            _multiplier = Smoothing / (averagePeriod + 1);

            var pricesToAverage = candles.Select(x => new KeyValuePair<DateTime, decimal>(x.TimeStamp, x.Close.Value))
                .ToDictionary(key => key.Key, value => value.Value);

            return CalculateExponentialMovingAverage(pricesToAverage, averagePeriod);
        }

        private Dictionary<DateTime, decimal> CalculateExponentialMovingAverage(
            Dictionary<DateTime, decimal> prices, int movingAveragePeriod)
        {
            var exponentialMovingAverages = new Dictionary<DateTime, decimal>();
            var priceArray = prices.OrderBy(x => x.Key).ToArray();

            for (var i = 0; i < priceArray.Length; i++)
            {
                if (i == 0)
                {
                    exponentialMovingAverages.Add(priceArray[i].Key, priceArray[i].Value);
                    continue;
                }

                var yesterday = priceArray[i - 1].Key;
                var ema = priceArray[i].Value * _multiplier + exponentialMovingAverages[yesterday] * (1 - _multiplier);

                exponentialMovingAverages.Add(priceArray[i].Key, ema);
            }

            return exponentialMovingAverages;
        }
    }
}
