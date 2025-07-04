﻿using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos;

public class BalanceDto
{
    [JsonProperty("symbol")] public string Symbol { get; set; }

    [JsonProperty("available")] public string Available { get; set; }

    [JsonProperty("inOrder")] public string InOrder { get; set; }
}