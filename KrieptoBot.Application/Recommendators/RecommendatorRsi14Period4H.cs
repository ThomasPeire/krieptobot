using KrieptoBot.Application.Indicators;
using KrieptoBot.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KrieptoBot.Application.Recommendators;

public class RecommendatorRsi14Period4H(
    IExchangeService exchangeService,
    IRsi rsiIndicator,
    ITradingContext tradingContext,
    ILogger<RecommendatorRsi14Period4H> logger,
    IOptions<RecommendatorSettings> recommendatorSettings)
    : RecommendatorRsiBase(exchangeService,
        rsiIndicator, tradingContext, logger, "4h", 14, recommendatorSettings.Value);