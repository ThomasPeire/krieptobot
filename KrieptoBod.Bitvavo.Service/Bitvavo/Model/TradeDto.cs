using Newtonsoft.Json;

namespace KrieptoBod.Exchange.Bitvavo.Model
{
    public class TradeDto
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }
    }
}
