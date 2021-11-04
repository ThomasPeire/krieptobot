using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14PeriodInterval : RecommendatorRsiBase
    {
        protected override float SellRecommendationWeight => 1F;
        protected override float BuyRecommendationWeight => 1F;

        public RecommendatorRsi14PeriodInterval(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14PeriodInterval> logger) : base(exchangeService,
            rsiIndicator, tradingContext, logger,
            tradingContext.Interval, 14)
        {
        }
    }
}
