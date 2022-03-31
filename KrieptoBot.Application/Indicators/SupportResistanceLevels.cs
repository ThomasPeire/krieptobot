using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Domain.Recommendation.ValueObjects;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.FSharp.Collections;

namespace KrieptoBot.Application.Indicators
{
    public class SupportResistanceLevels : ISupportResistanceLevels
    {
        private const int WindowLength = 5;

        public IEnumerable<SupportResistanceLevel> Calculate(IEnumerable<Candle> candles)
        {
            var averageHighLowDifference = candles.Select(x => x.High - x.Low).Average();

            var fractals = CreateFractals(candles);
            var rawLevels = GetRawLevels(fractals).ToList();

            return rawLevels
                .OrderBy(x => x.From)
                .Where(level =>
                    !rawLevels.Any(x => x.From < level.From && Math.Abs(level - x) <= averageHighLowDifference/2))
                .ToList();
        }

        private static IEnumerable<SupportResistanceLevel> GetRawLevels(IEnumerable<Candle[]> fractals)
        {
            var levels = new List<SupportResistanceLevel>();

            foreach (var fractal in fractals)
            {
                if (FractalTryToFindSupport(fractal, out var supportLevel))
                {
                    levels.Add(supportLevel);
                }

                if (FractalTryToFindResistance(fractal, out var resistanceLevel))
                {
                    levels.Add(resistanceLevel);
                }
            }

            return levels.Distinct();
        }

        private static bool FractalTryToFindSupport(IReadOnlyList<Candle> fractal,
            out SupportResistanceLevel supportLevel)
        {
            supportLevel = null;
            var firstCandleHasHigherLowThanSecondCandle = fractal[0].Low > fractal[1].Low;
            var secondCandleHasHigherLowThanThirdCandle = fractal[1].Low > fractal[2].Low;
            var thirdCandleHasLowerLowThanFourthCandle = fractal[2].Low < fractal[3].Low;

            var fourthCandleHasLowerLowThanFifthCandle = fractal[3].Low < fractal[4].Low;
            if (firstCandleHasHigherLowThanSecondCandle && secondCandleHasHigherLowThanThirdCandle &&
                thirdCandleHasLowerLowThanFourthCandle && fourthCandleHasLowerLowThanFifthCandle)
            {
                supportLevel = new SupportResistanceLevel(fractal[2].Low, fractal[2].TimeStamp);
                return true;
            }

            return false;
        }

        private static bool FractalTryToFindResistance(IReadOnlyList<Candle> fractal,
            out SupportResistanceLevel resistanceLevel)
        {
            resistanceLevel = null;

            var firstCandleHasLowerHighThanSecondCandle = fractal[0].High < fractal[1].High;
            var secondCandleHasLowerHighThanThirdCandle = fractal[1].High < fractal[2].High;
            var thirdCandleHasHigherHighThanFourthCandle = fractal[2].High > fractal[3].High;
            var fourthCandleHasHigherHighThanFifthCandle = fractal[3].High > fractal[4].High;

            if (firstCandleHasLowerHighThanSecondCandle && secondCandleHasLowerHighThanThirdCandle &&
                thirdCandleHasHigherHighThanFourthCandle && fourthCandleHasHigherHighThanFifthCandle)
            {
                resistanceLevel = new SupportResistanceLevel(fractal[2].High, fractal[2].TimeStamp);
                return true;
            }

            return false;
        }

        private static IEnumerable<Candle[]> CreateFractals(IEnumerable<Candle> candles)
        {
            return SeqModule.Windowed(WindowLength, candles);
        }
    }
}
