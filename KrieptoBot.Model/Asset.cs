using System.Collections.Generic;

namespace KrieptoBot.Model
{
    public class Asset
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Decimals { get; set; }
        public decimal DepositFee { get; set; }
        public int DepositConfirmations { get; set; }
        public string DepositStatus { get; set; }
        public decimal WithdrawalFee { get; set; }
        public decimal WithdrawalMinAmount { get; set; }
        public string WithdrawalStatus { get; set; }
        public List<string> Networks { get; set; }
        public string Message { get; set; }
    }
}
