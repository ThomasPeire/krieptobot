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
using Snapshooter.NUnit;

namespace KrieptoBot.Tests.Application.Indicators
{
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
                }.ConvertToKrieptoBotModel());
        }

        [Test]
        public void Rsi_Should_CalculateRsi()
        {
            var datetimeFrom = new DateTime(2021, 08, 13);

            var candlesToWorkWith = _candles
                .Where(x => x.TimeStamp >= datetimeFrom && x.TimeStamp <= datetimeFrom.AddDays(1))
                .OrderBy(x => x.TimeStamp);

            var rsiValues = new Rsi().Calculate(candlesToWorkWith, 14);

            rsiValues.Should().MatchSnapshot();
        }
    }
}
