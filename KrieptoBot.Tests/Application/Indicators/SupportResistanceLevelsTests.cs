using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using KrieptoBot.Application.Indicators;
using KrieptoBot.Domain.Trading.ValueObjects;
using KrieptoBot.Infrastructure.Bitvavo.Dtos;
using KrieptoBot.Infrastructure.Bitvavo.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Plotly.NET;
using Plotly.NET.LayoutObjects;
using Plotly.NET.TraceObjects;
using Snapshooter.NUnit;

namespace KrieptoBot.Tests.Application.Indicators
{
    internal class SupportResistanceLevelsTests
    {
        private IEnumerable<Candle> _candles;

        [SetUp]
        public void Setup()
        {
            InitCandles();
        }

        private void InitCandles()
        {
            var candlesJson = File.ReadAllText(@"./MockData/Bitvavo/candles_btc-eur_4h.json");
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
                }.ConvertToKrieptoBotModel());
        }

        [Test]
        public void CalculateWithStrength_Should_DetermineSupportAndResistanceLevelsWithStrength()
        {
            var datetimeFrom = new DateTime(2021, 11, 1);
            var dateTimeTo = new DateTime(2021, 11, 30);

            var candlesToWorkWith = _candles
                .Where(x => x.TimeStamp >= datetimeFrom && x.TimeStamp <= dateTimeTo)
                .OrderBy(x => x.TimeStamp);

            var supportLevels = new SupportResistanceLevels().CalculateLevelsWithStrength(candlesToWorkWith);

            var levelsToPlot = supportLevels.Keys.Select(x => new List<Tuple<DateTime, decimal>>
            {
                new(candlesToWorkWith.Select(x => x.TimeStamp).Min(), x.Value),
                new(candlesToWorkWith.Select(x => x.TimeStamp).Max(), x.Value)
            });

            var charts = levelsToPlot.Select(levelToPlot =>
                    Chart2D.Chart.Line<DateTime, decimal, string>(levelToPlot, MarkerColor: Color.fromString("Gray")))
                .ToList();
            charts.Add(Chart2D.Chart.Candlestick<decimal, decimal, decimal, decimal, DateTime, string>(
                candlesToWorkWith.Select(x => x.Open.Value), candlesToWorkWith.Select(x => x.High.Value),
                candlesToWorkWith.Select(x => x.Low.Value), candlesToWorkWith.Select(x => x.Close.Value),
                candlesToWorkWith.Select(x => x.TimeStamp)));

            Chart
                .Combine(charts)
                .WithConfig(Config.init(Responsive: true, Autosizable: true)).WithSize(2000, 1200)
                .Show();
        }
    }
}
