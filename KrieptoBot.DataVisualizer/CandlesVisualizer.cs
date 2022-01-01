using System;
using System.Collections.Generic;
using System.Linq;
using KrieptoBot.Domain.Trading.ValueObjects;
using Plotly.NET;

namespace KrieptoBot.DataVisualizer
{
    public class CandlesVisualizer : ICandlesVisualizer
    {
        public GenericChart.GenericChart Visualize(IEnumerable<Candle> candles)
        {
            return Chart2D.Chart.Candlestick<decimal, decimal, decimal, decimal, DateTime, string>(
                candles.Select(x => x.Open.Value), candles.Select(x => x.High.Value),
                candles.Select(x => x.Low.Value), candles.Select(x => x.Close.Value),
                candles.Select(x => x.TimeStamp));
        }


    }
}
