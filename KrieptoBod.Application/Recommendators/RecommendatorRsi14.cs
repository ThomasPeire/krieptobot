using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KrieptoBod.Model;

namespace KrieptoBod.Application.Recommendators
{
    public class RecommendatorRsi14 : RecommendatorBase
    {
        public override float Weight => 0.7F;

        private readonly IExchangeService _exchangeService;

        public RecommendatorRsi14(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market);

            var upsAndDownMoves = CalculateUpAndDownMoves(candles);

            var avgUpsAndAvgDowns = CalculateSimpleRSI(upsAndDownMoves);

            var rsiValues = CalculateRSI(avgUpsAndAvgDowns);
            
            return new RecommendatorScore { Score = .0F };
        }

        private Dictionary<DateTime, decimal> CalculateRSI(Dictionary<DateTime, (decimal avgsUp, decimal avgsDown)> avgUpsAndAvgDowns)
        {
            return new(avgUpsAndAvgDowns.Select(x =>
                new KeyValuePair<DateTime, decimal>(x.Key, 100 - (100 / (1 + (x.Value.avgsUp / x.Value.avgsDown))))));
        }

        private Dictionary<DateTime, (decimal avgsUp, decimal avgsDown)> CalculateSimpleRSI(Dictionary<DateTime, (decimal ups, decimal downs)> upsAndDownMoves)
        {
            var avgDowns = new List<decimal>();
            var avgUps = new List<decimal>();

            var movingAverages = new Dictionary<DateTime, (decimal, decimal)>();
            var upsAndDownMovesArray = upsAndDownMoves.OrderBy(x => x.Key).ToArray();

            // start with i=14 since we need the previous 14 values to calculate
            for (var i = 14; i < upsAndDownMovesArray.Length; i++)
            {
                var last14Values = upsAndDownMovesArray.Skip(i).Take(-14).Select(x => x.Value).ToList();

                var avgUp = last14Values.Average(x => x.ups);
                var avgDown = last14Values.Average(x => x.downs);

                movingAverages.Add(upsAndDownMovesArray[i].Key, (avgUp, avgDown));
            }

            return movingAverages;
        }

        private Dictionary<DateTime, (decimal ups, decimal downs)> CalculateUpAndDownMoves(IEnumerable<Candle> candles)
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
