using System;
using KrieptoBod.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KrieptoBod.Application.Indicators;
using KrieptoBod.Infrastructure.Bitvavo.Dtos;
using KrieptoBod.Infrastructure.Bitvavo.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrieptoBod.Tests.Application.Indicators
{
    class RsiTests
    {
        private IEnumerable<Candle> _candles;
        
        [SetUp]
        public void Setup()
        {
            InitCandles();
        }

        private void InitCandles()
        {
            var candlesJson = System.IO.File.ReadAllText(@"./Mocks/Bitvavo/Data/candles_btc-eur.json");
            var deserializedCandles = JsonConvert.DeserializeObject(candlesJson) as JArray;
            _candles = deserializedCandles.Select(x =>
                new CandleDto
                {
                    TimeStamp = DateTime.UnixEpoch.AddMilliseconds(x.Value<long>(0)),
                    Open = x.Value<decimal>(1),
                    High = x.Value<decimal>(2),
                    Low = x.Value<decimal>(3),
                    Close = x.Value<decimal>(4),
                    Volume = x.Value<decimal>(5),
                }.ConvertToKrieptoBodModel());
        }

        [Test]
        public void Rsi_Should_CalculateRsi()
        {
            var datetimeFrom = new DateTime(2021, 08, 13);
            Debug.WriteLine(_candles.OrderBy(x => x.TimeStamp).Last().TimeStamp);
            Debug.WriteLine(_candles.OrderBy(x => x.TimeStamp).First().TimeStamp);
            var candlesFromFebruary = _candles.Where(x => x.TimeStamp >= datetimeFrom && x.TimeStamp <= datetimeFrom.AddDays(1));

            var rsiValues = new Rsi().Calculate(candlesFromFebruary, 14);

            foreach (var keyValuePair in rsiValues)
            {
               Debug.WriteLine(keyValuePair.Key.ToString() +" "+ keyValuePair.Value); 
            }
        }
    }
}
