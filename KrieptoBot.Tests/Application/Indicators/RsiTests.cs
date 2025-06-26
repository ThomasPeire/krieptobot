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

namespace KrieptoBot.Tests.Application.Indicators;

internal class RsiTests
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
    public void Rsi_Should_CalculateRsi()
    {
        var datetimeFrom = new DateTime(2021, 11, 01);
        var dateTimeTo = datetimeFrom.AddHours(8);

        var candlesToWorkWith = _candles
            .Where(x => x.TimeStamp >= datetimeFrom && x.TimeStamp <= dateTimeTo)
            .OrderBy(x => x.TimeStamp).ToList();

        var rsiValues = new Rsi(new ExponentialMovingAverage()).Calculate(candlesToWorkWith, 14).RsiValues;

        rsiValues = candlesToWorkWith.ToDictionary(x => x.TimeStamp,
            x => rsiValues.TryGetValue(x.TimeStamp, out var rsiValue) ? rsiValue : 0);
        Snapshot.Match(rsiValues);

#if DEBUG
        var candleVisualizer = new CandlesVisualizer();
        var candleChart = candleVisualizer.Visualize(candlesToWorkWith);

        candleChart = candleChart.AddSubChartLine(rsiValues);
        candleChart.WithSize(1920, 1080).WithConfig(Config.init(Responsive: true)).Show();
#endif
    }
}