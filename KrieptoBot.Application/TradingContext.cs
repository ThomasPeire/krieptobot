using System;
using System.Collections.Generic;

namespace KrieptoBot.Application
{
    public class TradingContext : ITradingContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public TradingContext(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            SetCurrentTime();
        }

        public string Interval { get; private set; } = "5m";
        public IEnumerable<string> MarketsToWatch { get; private set; } = new List<string> { "BTC-EUR" };
        public DateTime CurrentTime { get; private set; }
        public int BuyMargin { get; private set; } = 30;
        public int SellMargin { get; private set; } = -30;

        public bool IsSimulation { get; private set; } = true;

        public TradingContext SetCurrentTime()
        {
            CurrentTime = _dateTimeProvider.UtcDateTimeNow();
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

        public TradingContext SetBuyMargin(int buyMargin)
        {
            BuyMargin = buyMargin;
            return this;
        }

        public TradingContext SetSellMargin(int sellMargin)
        {
            SellMargin = sellMargin;
            return this;
        }

        public TradingContext SetIsSimulation(bool isSimulation)
        {
            IsSimulation = isSimulation;
            return this;
        }
    }
}
