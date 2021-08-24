using KrieptoBod.Application.Indicators;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBod.Application.Recommendators
{
    public class RecommendatorRsi14 : RecommendatorBase
    {
        public override float Weight => 1F;

        private readonly IExchangeService _exchangeService;
        private readonly IRsi _rsiIndicator;

        public RecommendatorRsi14(IExchangeService exchangeService, IRsi rsiIndicator)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market);

            var rsiValues = _rsiIndicator.Calculate(candles, 14);

            var currentRsiValue = rsiValues.OrderBy(x => x.Key).Last();

            return EvaluateRsiValue(currentRsiValue.Value);
        }

        private RecommendatorScore EvaluateRsiValue(decimal rsiValue)
        {
            var rsiRecommendation = rsiValue switch
            {
                >= 70 => new RecommendatorScore() { Score = (float)(rsiValue - 70) },
                <= 30 => new RecommendatorScore() { Score = (float)(rsiValue - 30) },
                _ => new RecommendatorScore() { Score = 0F }
            };

            return rsiRecommendation / 30;
        }

        
    }
}
