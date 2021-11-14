using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14Period4H : RecommendatorRsiBase
    {
        public RecommendatorRsi14Period4H(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14Period4H> logger) : base(exchangeService,
            rsiIndicator, tradingContext, logger, "4h", 14)
        {
        }

        protected override decimal SellRecommendationWeight => 1.3m;
        protected override decimal BuyRecommendationWeight => 1.3m;
    }
}