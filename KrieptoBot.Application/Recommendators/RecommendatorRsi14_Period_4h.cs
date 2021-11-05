using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14Period4H : RecommendatorRsiBase
    {
        protected override decimal SellRecommendationWeight => 2;
        protected override decimal BuyRecommendationWeight => 2;

        public RecommendatorRsi14Period4H(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14Period4H> logger) : base(exchangeService,
            rsiIndicator, tradingContext, logger, "4h", 14)
        {
        }
    }
}
