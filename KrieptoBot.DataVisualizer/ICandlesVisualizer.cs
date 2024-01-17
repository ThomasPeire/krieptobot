using KrieptoBot.Domain.Trading.ValueObjects;
using Plotly.NET;

namespace KrieptoBot.DataVisualizer
{
    public interface ICandlesVisualizer
    {
        GenericChart.GenericChart Visualize(IEnumerable<Candle> candles);
    }
}
