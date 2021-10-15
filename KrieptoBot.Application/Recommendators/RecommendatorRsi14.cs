using KrieptoBot.Application.Indicators;
using System.Linq;
using System.Threading.Tasks;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14 : RecommendatorBase
    {
        public override float SellRecommendationWeight => 1F;
        public override float BuyRecommendationWeight => 1F;

        private readonly IExchangeService _exchangeService;
        private readonly IRsi _rsiIndicator;
        private readonly ITradingContext _tradingContext;

        public RecommendatorRsi14(IExchangeService exchangeService, IRsi rsiIndicator, ITradingContext tradingContext)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
            _tradingContext = tradingContext;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market, _tradingContext.Interval, 20, end: _tradingContext.CurrentTime);

            var rsiValues = _rsiIndicator.Calculate(candles, 14);

            var currentRsiValue = rsiValues.OrderBy(x => x.Key).Last();

            return EvaluateRsiValue(currentRsiValue.Value);
        }

        private RecommendatorScore EvaluateRsiValue(decimal rsiValue)
        {
            var rsiRecommendation = new RecommendatorScore() { Score = (float)((50 - rsiValue) / 100) * 2 };

            return rsiRecommendation;
        }


    }
}
