using KrieptoBot.Application.Indicators;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14Period4H : RecommendatorRsiBase
    {
        public override float SellRecommendationWeight => 1F;
        public override float BuyRecommendationWeight => 1F;

        public RecommendatorRsi14Period4H(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14Period4H> logger) : base(exchangeService,
            rsiIndicator, tradingContext, logger, "4h", 14)
        {
        }
    }
}
