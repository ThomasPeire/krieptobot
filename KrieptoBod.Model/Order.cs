using System;
using System.Collections.Generic;

namespace KrieptoBod.Model
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Market { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Status { get; set; }
        public string Side { get; set; }
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountRemaining { get; set; }
        public decimal Price { get; set; }
        public decimal AmountQuote { get; set; }
        public decimal AmountQuoteRemaining { get; set; }
        public decimal OnHold { get; set; }
        public string OnHoldCurrency { get; set; }
        public decimal TriggerPrice { get; set; }
        public decimal TriggerAmount { get; set; }
        public string TriggerType { get; set; }
        public string TriggerReference { get; set; }
        public decimal FilledAmount { get; set; }
        public decimal FilledAmountQuote { get; set; }
        public decimal FeePaid { get; set; }
        public string FeeCurrency { get; set; }
        public IEnumerable<Fill> Fills { get; set; }
        public string SelfTradePrevention { get; set; }
        public bool Visible { get; set; }
        public string TimeInForce { get; set; }
        public bool PostOnly { get; set; }
        public bool DisableMarketProtection { get; set; }

    }

    public class Fill
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public bool Taker { get; set; }
        public decimal Fee { get; set; }
        public string FeeCurrency { get; set; }
        public bool Settled { get; set; }
    }

}
