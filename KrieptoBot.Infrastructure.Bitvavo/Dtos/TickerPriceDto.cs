using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos;

public class TickerPriceDto
{
    [JsonProperty("market")] public string Market { get; set; }

    [JsonProperty("price")] public string Price { get; set; }
}