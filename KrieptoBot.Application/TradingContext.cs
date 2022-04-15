using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public class TradingContext : ITradingContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TradingContext(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            var _ = SetCurrentTime().Result;
        }

        public string Interval { get; private set; } = "5m";
        public IEnumerable<string> MarketsToWatch { get; private set; } = new List<string> { "BTC-EUR" };
        public DateTime CurrentTime { get; private set; }
        public decimal BuyMargin { get; private set; } = 0.5m;
        public decimal SellMargin { get; private set; } = -0.5m;

        public bool IsSimulation { get; private set; } = true;

        public int PollingIntervalInMinutes { get; private set; } = 5;

        public int BuyCoolDownPeriodInMinutes { get; private set; } = 5;

        public async Task<TradingContext> SetCurrentTime()
        {
            CurrentTime = await _dateTimeProvider.UtcDateTimeNow();
            return this;
        }

        public TradingContext SetMarketsToWatch(IEnumerable<string> markets)
        {
            MarketsToWatch = markets;
            return this;
        }

        public TradingContext SetInterval(string interval)
        {
            Interval = interval;
            return this;
        }

        public TradingContext SetBuyMargin(decimal buyMargin)
        {
            BuyMargin = buyMargin;
            return this;
        }

        public TradingContext SetSellMargin(decimal sellMargin)
        {
            SellMargin = sellMargin;
            return this;
        }

        public TradingContext SetIsSimulation(bool isSimulation)
        {
            IsSimulation = isSimulation;
            return this;
        }

        public TradingContext SetPollingInterval(int pollingIntervalInMinutes)
        {
            PollingIntervalInMinutes = pollingIntervalInMinutes;
            return this;
        }

        public TradingContext SetBuyCoolDownPeriod(int buyCoolDownPeriodInMinutes)
        {
            BuyCoolDownPeriodInMinutes = buyCoolDownPeriodInMinutes;
            return this;
        }
    }
}
