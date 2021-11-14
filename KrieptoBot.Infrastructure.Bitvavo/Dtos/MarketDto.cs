using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos
{
    public class MarketDto
    {
        [JsonProperty("market")] public string MarketName { get; set; }

        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("base")] public string Base { get; set; }

        [JsonProperty("quote")] public string Quote { get; set; }

        [JsonProperty("pricePrecision")] public string PricePrecision { get; set; }

        [JsonProperty("minOrderInQuoteAsset")] public string MinOrderInQuoteAsset { get; set; }

        [JsonProperty("minOrderInBaseAsset")] public string MinOrderInBaseAsset { get; set; }

        [JsonProperty("orderTypes")] public List<string> OrderTypes { get; set; }
    }
}