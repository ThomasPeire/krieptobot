using KrieptoBot.Domain.Trading.ValueObjects;
using Plotly.NET;

namespace KrieptoBot.DataVisualizer
{
    public class CandlesVisualizer : ICandlesVisualizer
    {
        public GenericChart.GenericChart Visualize(IEnumerable<Candle> candles)
        {
            var candlesArray = candles as Candle[] ?? candles.ToArray();
            return Chart2D.Chart.Candlestick<decimal, decimal, decimal, decimal, DateTime, string>(
                candlesArray.Select(x => x.Open.Value), candlesArray.Select(x => x.High.Value),
                candlesArray.Select(x => x.Low.Value), candlesArray.Select(x => x.Close.Value),
                candlesArray.Select(x => x.TimeStamp));
        }


    }
}
