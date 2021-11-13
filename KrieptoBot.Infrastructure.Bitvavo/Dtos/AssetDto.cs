using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrieptoBot.Infrastructure.Bitvavo.Dtos
{
    public class AssetDto
    {
        [JsonProperty("symbol")] public string Symbol { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("decimals")] public int Decimals { get; set; }

        [JsonProperty("depositFee")] public string DepositFee { get; set; }

        [JsonProperty("depositConfirmations")] public int DepositConfirmations { get; set; }

        [JsonProperty("depositStatus")] public string DepositStatus { get; set; }

        [JsonProperty("withdrawalFee")] public string WithdrawalFee { get; set; }

        [JsonProperty("withdrawalMinAmount")] public string WithdrawalMinAmount { get; set; }

        [JsonProperty("withdrawalStatus")] public string WithdrawalStatus { get; set; }

        [JsonProperty("networks")] public List<string> Networks { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }
}