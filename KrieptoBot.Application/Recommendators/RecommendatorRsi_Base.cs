using System.Linq;
using System.Threading.Tasks;
using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public abstract class RecommendatorRsiBase : RecommendatorBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly IRsi _rsiIndicator;
        private readonly ITradingContext _tradingContext;
        private readonly ILogger<RecommendatorRsiBase> _logger;
        private readonly string _tradingContextInterval;
        private readonly int _periodToAverage;

        protected RecommendatorRsiBase(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsiBase> logger, string tradingContextInterval, int periodToAverage)
        {
            _exchangeService = exchangeService;
            _rsiIndicator = rsiIndicator;
            _tradingContext = tradingContext;
            _logger = logger;
            _tradingContextInterval = tradingContextInterval;
            _periodToAverage = periodToAverage;
        }

        protected override async Task<RecommendatorScore> CalculateRecommendation(string market)
        {
            var candles = await _exchangeService.GetCandlesAsync(market, _tradingContextInterval, _periodToAverage + 2,
                end: _tradingContext.CurrentTime);

            var rsiValues = _rsiIndicator.Calculate(candles, _periodToAverage);

            var currentRsiValue = rsiValues.OrderBy(x => x.Key).Last();

            _logger.LogInformation("{Recommendator}: RSI{PeriodToAverage} for market {Market} is {RsiValue}", this.GetType().Name, _periodToAverage, market, currentRsiValue);

            var recommendatorScore = EvaluateRsiValue(currentRsiValue.Value);

            _logger.LogInformation("{Recommendator}: Score {Score} for market {Market}", this.GetType().Name, recommendatorScore.Score, market);

            return recommendatorScore;
        }

        private RecommendatorScore EvaluateRsiValue(decimal rsiValue)
        {
            var rsiRecommendation = new RecommendatorScore() { Score = (float)((50 - rsiValue) / 100) * 2 };

            return rsiRecommendation;
        }
    }
}
