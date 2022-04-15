using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos
{
    public class TimeDto
    {
        [JsonProperty("time")] public long TimeInMilliseconds { get; set; }
    }
}
