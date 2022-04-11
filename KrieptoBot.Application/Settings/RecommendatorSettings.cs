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
    }
}
