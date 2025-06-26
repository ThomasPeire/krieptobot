using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KrieptoBot.Application.Indicators;
using KrieptoBot.DataVisualizer;
using KrieptoBot.DataVisualizer.Extensions;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Plotly.NET;
using Snapshooter.NUnit;
using Color = System.Drawing.Color;

namespace KrieptoBot.Tests.Application.Indicators;

internal class MovingAverageTests
{
    private IEnumerable<Candle> _candles;

    [SetUp]
    public void Setup()
    {
        InitCandles();
    }

    private void InitCandles()
    {
        var candlesJson = File.ReadAllText(@"./MockData/Bitvavo/candles_btc-eur.json");
        var deserializedCandles = JsonConvert.DeserializeObject(candlesJson) as JArray;
        _candles = deserializedCandles.Select(x =>
            new CandleDto
            {
                TimeStamp = x.Value<long>(0),
                Open = x.Value<decimal>(1),
                High = x.Value<decimal>(2),
                Low = x.Value<decimal>(3),
                Close = x.Value<decimal>(4),
                Volume = x.Value<decimal>(5)
            }.ConvertToKrieptoBotModel()).DistinctBy(x => x.TimeStamp);
    }

    [Test]
    public void MovingAverage_Should_CalculateMovingAverage()
    {
        var datetimeFrom = new DateTime(2021, 11, 01);
        var dateTimeTo = datetimeFrom.AddHours(8);

        var candlesToWorkWith = _candles
            .Where(x => x.TimeStamp >= datetimeFrom && x.TimeStamp <= dateTimeTo)
            .OrderBy(x => x.TimeStamp);

        var values5 = new MovingAverage().Calculate(candlesToWorkWith, 5);
        var values10 = new MovingAverage().Calculate(candlesToWorkWith, 14);

        values5 = candlesToWorkWith.ToDictionary(x => x.TimeStamp,
            x => values5.TryGetValue(x.TimeStamp, out var value) ? value : 0);

        Snapshot.Match(values5);
#if DEBUG
        var candleVisualizer = new CandlesVisualizer();
        var candleChart = candleVisualizer.Visualize(candlesToWorkWith);

        candleChart = candleChart.AddLineChart(values5, Color.Yellow);
        candleChart = candleChart.AddLineChart(values10, Color.Blue);
        candleChart.WithSize(1920, 1080).WithConfig(Config.init(Responsive: true)).Show();
#endif
    }
}