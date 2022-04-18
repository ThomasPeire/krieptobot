using System.Collections.Generic;

namespace KrieptoBot.Application.Settings
{
    public class RecommendatorSettings
    {
        public Dictionary<string, decimal> BuyRecommendationWeights { get; set; }
        public Dictionary<string, decimal> SellRecommendationWeights { get; set; }
        public decimal ProfitRecommendatorProfitThresholdPct { get; set; }
        public decimal ProfitRecommendatorLossThresholdPct { get; set; }
        public int DownTrendRecommendatorNumberOfConsecutiveCandles { get; set; }
        public string DownTrendRecommendatorInterval { get; set; }
        public int RsiEmaRecommendatorEmaPeriod { get; set; }
        public int RsiEmaRecommendatorRsiPeriod { get; set; }
        public decimal RsiEmaRecommendatorBuySignalThreshold { get; set; }
        public decimal RsiEmaRecommendatorSellSignalThreshold { get; set; }
    }
}
