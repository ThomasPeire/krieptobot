using System.Collections.Generic;

namespace KrieptoBot.Application.Settings
{
    public class RecommendatorSettings
    {
        public Dictionary<string, decimal> BuyRecommendationWeights { get; set; }
        public Dictionary<string, decimal> SellRecommendationWeights { get; set; }
    }
}