using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos
{
    public class OrderDto
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("amountRemaining")]
        public string AmountRemaining { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("amountQuote")]
        public string AmountQuote { get; set; }

        [JsonProperty("amountQuoteRemaining")]
        public string AmountQuoteRemaining { get; set; }

        [JsonProperty("onHold")]
        public string OnHold { get; set; }

        [JsonProperty("onHoldCurrency")]
        public string OnHoldCurrency { get; set; }

        [JsonProperty("triggerPrice")]
        public string TriggerPrice { get; set; }

        [JsonProperty("triggerAmount")]
        public string TriggerAmount { get; set; }

        [JsonProperty("triggerType")]
        public string TriggerType { get; set; }

        [JsonProperty("triggerReference")]
        public string TriggerReference { get; set; }

        [JsonProperty("filledAmount")]
        public string FilledAmount { get; set; }

        [JsonProperty("filledAmountQuote")]
        public string FilledAmountQuote { get; set; }

        [JsonProperty("feePaid")]
        public string FeePaid { get; set; }

        [JsonProperty("feeCurrency")]
        public string FeeCurrency { get; set; }

        [JsonProperty("fills")]
        public List<FillDto> Fills { get; set; }

        [JsonProperty("selfTradePrevention")]
        public string SelfTradePrevention { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }

        [JsonProperty("postOnly")]
        public bool PostOnly { get; set; }

        [JsonProperty("disableMarketProtection")]
        public bool DisableMarketProtection { get; set; }

    }

    public class FillDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("taker")]
        public bool Taker { get; set; }

        [JsonProperty("fee")]
        public string Fee { get; set; }

        [JsonProperty("feeCurrency")]
        public string FeeCurrency { get; set; }

        [JsonProperty("settled")]
        public bool Settled { get; set; }
    }

}
