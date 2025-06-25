using System.Collections.Generic;
using KrieptoBot.Domain.Trading.ValueObjects;
using Plotly.NET;

namespace KrieptoBot.DataVisualizer
{
    interface ICandlesVisualizer
    {
        GenericChart Visualize(IEnumerable<Candle> candles);
    }
}
