﻿using System;
using System.Collections.Generic;

namespace KrieptoBot.Application
{
    public class TradingContext: ITradingContext
    {
        public string Interval { get; private set; }
        public IEnumerable<string> MarketsToWatch { get; private set; }
        public DateTime CurrentTime { get; private set; } = DateTime.UtcNow;

        public int BuyMargin { get; private set; }
        public int SellMargin { get; private set; }

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
    }
}