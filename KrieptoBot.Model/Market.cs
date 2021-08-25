using System.Collections.Generic;

namespace KrieptoBot.Model
{
    public class Market
    {
        public string MarketName { get; set; }
        public string Status { get; set; }
        public string Base { get; set; }
        public string Quote { get; set; }
        public int PricePrecision { get; set; }
        public decimal MinOrderInQuoteAsset { get; set; }
        public decimal MinOrderInBaseAsset { get; set; }
        public List<string> OrderTypes { get; set; }
    }
}
