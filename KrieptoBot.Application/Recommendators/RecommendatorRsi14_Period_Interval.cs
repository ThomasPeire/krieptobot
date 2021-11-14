using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14PeriodInterval : RecommendatorRsiBase
    {
        public RecommendatorRsi14PeriodInterval(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14PeriodInterval> logger) : base(exchangeService,
            rsiIndicator, tradingContext, logger,
            tradingContext.Interval, 14)
        {
        }

        protected override decimal SellRecommendationWeight => 1;
        protected override decimal BuyRecommendationWeight => 1;
    }
}