﻿using System;
using System.Collections.Generic;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application.Indicators;

public interface IMovingAverage
{
    Dictionary<DateTime, decimal> Calculate(IEnumerable<Candle> candles, int averagePeriod);
}