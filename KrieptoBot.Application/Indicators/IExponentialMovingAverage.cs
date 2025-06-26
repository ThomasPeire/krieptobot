using System;
using System.Collections.Generic;

namespace KrieptoBot.Application.Indicators;

public interface IExponentialMovingAverage
{
    Dictionary<DateTime, decimal> Calculate(Dictionary<DateTime, decimal> prices, int averagePeriod);
}