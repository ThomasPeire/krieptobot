using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Application.Indicators.Results;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators
{
    public class Rsi : IRsi
    {
        private readonly IExponentialMovingAverage _exponentialMovingAverage;

        public Rsi(IExponentialMovingAverage exponentialMovingAverage)
        {
            _exponentialMovingAverage = exponentialMovingAverage;
        }

        public RsiResult Calculate(IEnumerable<Candle> candles, int averagePeriod)
        {
            var upsAndDownMoves = CalculateUpAndDownMoves(candles);

            var upAndDownAverages = CalculateMovingAverage(upsAndDownMoves, averagePeriod);

            var rsiValues = CalculateRsi(upAndDownAverages);

            return new RsiResult() { RsiValues = rsiValues };
        }

        public RsiResult CalculateWithEma(IEnumerable<Candle> candles, int averagePeriod, int emaPeriod)
        {
            var rsiValues = Calculate(candles, averagePeriod).RsiValues;
            var emaValues = _exponentialMovingAverage.Calculate(rsiValues, emaPeriod);

            return new RsiResult() { RsiValues = rsiValues, EmaValues = emaValues };
        }

        private static Dictionary<DateTime, decimal> CalculateRsi(
            Dictionary<DateTime, (decimal upAverage, decimal downAverage)> upAndDownAverages)
        {
            return new Dictionary<DateTime, decimal>(upAndDownAverages
                .Select(x =>
                    new KeyValuePair<DateTime, decimal>(x.Key, GetRsi(x.Value.upAverage, x.Value.downAverage))));
        }

        private static decimal GetRsi(decimal averageUp, decimal averageDown)
        {
            if (averageDown != 0)
            {
                var avg = (1 + averageUp / averageDown);
                if (avg != 0)
                {
                    return 100 - 100 / avg;
                }
            }

            return 50;
        }

        private static Dictionary<DateTime, (decimal upAverage, decimal downAverage)> CalculateMovingAverage(
            Dictionary<DateTime, (decimal up, decimal down)> upAndDownMoves, int movingAveragePeriod)
        {
            var movingAverages = new Dictionary<DateTime, (decimal avgerageUp, decimal averageDown)>();
            var upAndDownMovesArray = upAndDownMoves.OrderBy(x => x.Key).ToArray();

            var previousValues = GetStartingValues(upAndDownMovesArray, movingAveragePeriod).ToList();

            var previousAverageUp = previousValues.Average(x => x.up);
            var previousAverageDown = previousValues.Average(x => x.down);

            for (var i = movingAveragePeriod; i < upAndDownMovesArray.Length; i++)
            {
                var averageUp = (previousAverageUp * (movingAveragePeriod - 1) + upAndDownMovesArray[i].Value.up) /
                                movingAveragePeriod;
                var averageDown =
                    (previousAverageDown * (movingAveragePeriod - 1) + upAndDownMovesArray[i].Value.down) /
                    movingAveragePeriod;

                movingAverages.Add(upAndDownMovesArray[i].Key, (averageUp, averageDown));

                previousAverageUp = averageUp;
                previousAverageDown = averageDown;
            }

            return movingAverages;
        }

        private static IEnumerable<(decimal up, decimal down)> GetStartingValues(
            IEnumerable<KeyValuePair<DateTime, (decimal ups, decimal downs)>> upAndDownMovesArray,
            int movingAveragePeriod)
        {
            return upAndDownMovesArray.Take(movingAveragePeriod).Select(x => x.Value);
        }


        private static Dictionary<DateTime, (decimal up, decimal down)> CalculateUpAndDownMoves(
            IEnumerable<Candle> candles)
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
