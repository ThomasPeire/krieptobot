﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    public class RecommendatorRsi: RecommendatorBase
    {
        public override float Weight => 0.7F;

        private readonly IExchangeService _exchangeService;

        public RecommendatorRsi(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market);
            var closesArray = candles.Select(candle => (double)candle.Close).ToArray();
            
            var outArray = new double[closesArray.Count()];
            var outBegIdx = 0;
            try
            {
                TALib.Core.Rsi(closesArray, 0, closesArray.Count() - 1, outArray, out outBegIdx, out var outNbElement);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"exception calculating RSI 14: " + ex.Message);
            }

            var i = 0;
            var outCount = 0;
            foreach (var candle in candles)
            {
                if (i >= outBegIdx)
                {
                    Debug.WriteLine($"{candle.TimeStamp} - RSI: { outArray[outCount] } Close: {candle.Close}");
                    outCount++;
                }
                else
                {
                    Debug.WriteLine($"{candle.TimeStamp} - no data");
                }

                i++;
            }


            return new RecommendatorScore {Score = .0F};
        }
    }
}
