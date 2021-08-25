using KrieptoBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrieptoBot.Application.Indicators
{
    public class Rsi : IRsi
    {
        public Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int averagePeriod)
        {
            var upsAndDownMoves = CalculateUpAndDownMoves(candles);

            var upAndDownAverages = CalculateMovingAverage(upsAndDownMoves, averagePeriod);

            var rsiValues = CalculateRsi(upAndDownAverages);

            return rsiValues;
        }

        private Dictionary<DateTime, decimal> CalculateRsi(Dictionary<DateTime, (decimal upAverage, decimal downAverage)> upAndDownAverages)
        {
            return new(upAndDownAverages
                .Select(x =>
                    new KeyValuePair<DateTime, decimal>(x.Key, GetRSI(x.Value.upAverage, x.Value.downAverage))));
        }

        private decimal GetRSI(decimal averageUp, decimal averageDown)
        {
            return 100 - (100 / (1 + (averageUp / averageDown)));
        }

        private Dictionary<DateTime, (decimal upAverage, decimal downAverage)> CalculateMovingAverage(Dictionary<DateTime, (decimal ups, decimal downs)> upAndDownMoves, int movingAveragePeriod)
        {
            var movingAverages = new Dictionary<DateTime, (decimal avgerageUp, decimal averageDown)>();
            var upAndDownMovesArray = upAndDownMoves.OrderBy(x => x.Key).ToArray();

            // start with i=movingAveragePeriod-1 since we need the previous movingAveragePeriod values to calculate
            for (var i = movingAveragePeriod-1; i < upAndDownMovesArray.Length; i++)
            {
                var previousValues = GetValuesForAveragePeriod(upAndDownMovesArray, i - movingAveragePeriod, movingAveragePeriod).ToList();

                var upAverage = previousValues.Average(x => x.up);
                var downAverage = previousValues.Average(x => x.down);

                movingAverages.Add(upAndDownMovesArray[i].Key, (upAverage, downAverage));
            }

            return movingAverages;
        }

        private IEnumerable<(decimal up, decimal down)> GetValuesForAveragePeriod(IEnumerable<KeyValuePair<DateTime, (decimal ups, decimal downs)>> upAndDownMovesArray, int startIndex, int movingAveragePeriod)
        {
            return upAndDownMovesArray.Skip(startIndex+1).Take(movingAveragePeriod).Select(x => x.Value);
        }

        private Dictionary<DateTime, (decimal up, decimal down)> CalculateUpAndDownMoves(IEnumerable<Candle> candles)
        {
            var upsAndDowns = new Dictionary<DateTime, (decimal up, decimal down)>();

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
