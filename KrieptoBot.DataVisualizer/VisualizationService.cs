using System.Diagnostics;
using System.Text.Json;
using KrieptoBot.Application.Indicators;
using KrieptoBot.DataVisualizer.Extensions;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Hosting;
using Plotly.NET;
using Color = System.Drawing.Color;

namespace KrieptoBot.DataVisualizer
{
    public class VisualizationService : IHostedService
    {
        private readonly IHostApplicationLifetime _host;
        private readonly ICandlesVisualizer _visualizer;
        public VisualizationService(IHostApplicationLifetime host, ICandlesVisualizer visualizer)
        {
            _host = host;
            _visualizer = visualizer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var candles = InitCandles();
            var candleArray = candles as Candle[] ?? candles.ToArray();
            var dateTimeTo = candleArray.Max(x => x.TimeStamp);
            var datetimeFrom = dateTimeTo.AddDays(-1);
            
            var timeSlicedCandles = candleArray.Where(x => x.TimeStamp <= dateTimeTo && x.TimeStamp >= datetimeFrom).ToArray();

            var candlePrices = candleArray.ToDictionary(x => x.TimeStamp, x => x.Close.Value);

            var supportLevels = new SupportResistanceLevels().Calculate(timeSlicedCandles);
            
            var macdResult = new Macd(new ExponentialMovingAverage()).Calculate(timeSlicedCandles);

            var macdLine = timeSlicedCandles.ToDictionary(x => x.TimeStamp,
                x => macdResult.MacdLine.TryGetValue(x.TimeStamp, out var value) ? value : 0);
            var signalLine = timeSlicedCandles.ToDictionary(x => x.TimeStamp,
                x => macdResult.SignalLine.TryGetValue(x.TimeStamp, out var value) ? value : 0);
            var histogram = timeSlicedCandles.ToDictionary(x => x.TimeStamp,
                x => macdResult.Histogram.TryGetValue(x.TimeStamp, out var value) ? value : 0);

            var rsiEmaResult = new Rsi(new ExponentialMovingAverage()).CalculateWithEma(timeSlicedCandles, 17, 10);

            var chart = _visualizer.Visualize(timeSlicedCandles);
            chart = chart.AddSupportLevels(supportLevels, datetimeFrom, dateTimeTo);
            chart = chart.AddSubChartLine(macdLine).AddSubChartLine(signalLine);
                // .AddLineChart(signalLine, Color.Chartreuse).AddLineChart(histogram, Color.Gold);
            // chart = chart.AddSubChartLine(rsiEmaResult.RsiValues).AddLineChart(rsiEmaResult.EmaValues, Color.Red);

            chart.WithSize(1920, 1080).WithConfig(Config.init(Responsive: true)).Show();

            _host.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }

        private IEnumerable<Candle> InitCandles()
        {
            var candlesJson = File.ReadAllText(@"D:\XRP-EUR-5m.json");

            return
                JsonSerializer.Deserialize<Candle[]>(candlesJson)?
                    .DistinctBy(x => x.TimeStamp) ?? [];
        }
    }
}
