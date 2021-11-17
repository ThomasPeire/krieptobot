using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators
{
    public class RecommendatorRsi14PeriodInterval : RecommendatorRsiBase
    {
        public RecommendatorRsi14PeriodInterval(IExchangeService exchangeService, IRsi rsiIndicator,
            ITradingContext tradingContext, ILogger<RecommendatorRsi14PeriodInterval> logger,
            IOptions<RecommendatorSettings> recommendatorSettings) : base(exchangeService,
            rsiIndicator, tradingContext, logger,
            tradingContext.Interval, 14, recommendatorSettings.Value)
        {
        }
    }
}
