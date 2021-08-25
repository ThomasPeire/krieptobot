using KrieptoBot.Application.Indicators;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
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
            var rsiRecommendation = new RecommendatorScore() {Score = (float) (50 - rsiValue)};

            return rsiRecommendation / 50;
        }

        
    }
}
