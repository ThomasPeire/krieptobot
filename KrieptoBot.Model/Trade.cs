using System;

namespace KrieptoBot.Model
{
    public class Trade
    {
        public DateTime Timestamp { get; set; }
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
    }
}
