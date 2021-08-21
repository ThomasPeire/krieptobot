﻿using Newtonsoft.Json;
using System;

namespace KrieptoBod.Infrastructure.Bitvavo.Dtos
{
    public class CandleDto
    {
        [JsonProperty(Order = 0)]
        public DateTime TimeStamp { get; set; }
        [JsonProperty(Order = 1)]
        public decimal Open { get; set; }
        [JsonProperty(Order = 2)]
        public decimal High { get; set; }
        [JsonProperty(Order = 3)]
        public decimal Low { get; set; }
        [JsonProperty(Order = 4)]
        public decimal Close { get; set; }
        [JsonProperty(Order = 5)]
        public decimal Volume { get; set; }
    }
}