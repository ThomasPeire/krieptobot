using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.FSharp.Collections;

namespace KrieptoBot.Application.Indicators
{
    public class SupportResistanceLevels : ISupportResistanceLevels
    {
        private const int _windowLength = 5;

        public IDictionary<Price, int> CalculateLevelsWithStrength(IEnumerable<Candle> candles)
        {
            var averageHighLowDifference = candles.Select(x => x.High - x.Low).Average();

            var fractals = CreateFractals(candles);
            var rawLevels = GetRawLevels(fractals);

            var levels = AverageOutLevels(averageHighLowDifference/4, rawLevels);

            return levels.ToDictionary(x => x, x => 1);
        }

        private static IEnumerable<Price> AverageOutLevels(decimal averageHighLowDifference,
            IEnumerable<Price> levels)
        {
            var newLevels = levels
                .Select(currentLevel => GetNearLevelsForLevel(averageHighLowDifference, levels, currentLevel))
                .Select(nearLevels => new Price(nearLevels.Average(x => x.Value))).Distinct();

            return levels.Count() == newLevels.Count()
                ? newLevels.Distinct()
                : AverageOutLevels(averageHighLowDifference, newLevels).Distinct();
        }

        private static IEnumerable<Price> GetNearLevelsForLevel(decimal averageHighLowDifference,
            IEnumerable<Price> levels, Price currentLevel)
        {
            return levels.Where(x => Math.Abs(currentLevel - x) <= averageHighLowDifference);
        }

        private static IEnumerable<Price> GetRawLevels(IEnumerable<Candle[]> fractals)
        {
            var levels = new List<Price>();

            foreach (var fractal in fractals)
            {
                if (FractalTryToFindSupport(fractal, out var supportLevel))
                {
                    levels.Add(new Price(supportLevel));
                }

                if (FractalTryToFindResistance(fractal, out var resistanceLevel))
                {
                    levels.Add(new Price(resistanceLevel));
                }
            }

            return levels.Distinct();
        }

        private static bool FractalTryToFindSupport(IReadOnlyList<Candle> fractal, out decimal supportLevel)
        {
            supportLevel = 0;
            var firstCandleHasHigherLowThanSecondCandle = fractal[0].Low > fractal[1].Low;
            var secondCandleHasHigherLowThanThirdCandle = fractal[1].Low > fractal[2].Low;
            var thirdCandleHasLowerLowThanFourthCandle = fractal[2].Low < fractal[3].Low;

            var fourthCandleHasLowerLowThanFifthCandle = fractal[3].Low < fractal[4].Low;
            if (firstCandleHasHigherLowThanSecondCandle && secondCandleHasHigherLowThanThirdCandle &&
                thirdCandleHasLowerLowThanFourthCandle && fourthCandleHasLowerLowThanFifthCandle)
            {
                supportLevel = fractal[2].Low;
                return true;
            }

            return false;
        }

        private static bool FractalTryToFindResistance(IReadOnlyList<Candle> fractal, out decimal resistanceLevel)
        {
            resistanceLevel = 0;

            var firstCandleHasLowerHighThanSecondCandle = fractal[0].High < fractal[1].High;
            var secondCandleHasLowerHighThanThirdCandle = fractal[1].High < fractal[2].High;
            var thirdCandleHasHigherHighThanFourthCandle = fractal[2].High > fractal[3].High;
            var fourthCandleHasHigherHighThanFifthCandle = fractal[3].High > fractal[4].High;

            if (firstCandleHasLowerHighThanSecondCandle && secondCandleHasLowerHighThanThirdCandle &&
                thirdCandleHasHigherHighThanFourthCandle && fourthCandleHasHigherHighThanFifthCandle)
            {
                resistanceLevel = fractal[2].High;
                return true;
            }

            return false;
        }

        private static IEnumerable<Candle[]> CreateFractals(IEnumerable<Candle> candles)
        {
            return SeqModule.Windowed(_windowLength, candles);
        }
    }
}
