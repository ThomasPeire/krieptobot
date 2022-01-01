using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.DataCollector
{
    public class Collector : ICollector
    {
        private readonly IExchangeService _exchangeService;

        public Collector(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        public async Task CollectCandles(IEnumerable<string> markets, ICollection<string> intervals,
            DateTime fromDateTime, DateTime toDateTime)
        {
            foreach (var market in markets)
            {
                foreach (var interval in intervals)
                {
                    var tasks = new List<Task<IEnumerable<Candle>>>();

                    var intervalInMinutes = GetIntervalInMinutes(interval);

                    var currentStartDateTime = fromDateTime;
                    const int numberOfCandlesInOneCall = 1000;

                    while (currentStartDateTime <= toDateTime)
                    {
                        var startTime = currentStartDateTime;
                        var endTime =
                            new DateTime(Math.Min(
                                currentStartDateTime.AddMinutes(intervalInMinutes * numberOfCandlesInOneCall).Ticks,
                                toDateTime.Ticks));
                        Debug.WriteLine(startTime);
                        Debug.WriteLine(endTime);
                        tasks.Add(_exchangeService.GetCandlesAsync(market, interval, numberOfCandlesInOneCall,
                            startTime, endTime));

                        currentStartDateTime =
                            currentStartDateTime.AddMinutes(numberOfCandlesInOneCall * intervalInMinutes);
                    }

                    await Task.WhenAll(tasks);

                    var candles = new List<Candle>();
                    foreach (var task in tasks) candles.AddRange(await task);

                    var json = JsonSerializer.Serialize(candles.OrderBy(x => x.TimeStamp));
                    await File.WriteAllTextAsync($@"D:\{market}-{interval}.json", json);
                }
            }
        }

        private static int GetIntervalInMinutes(string interval)
        {
            return interval switch
            {
                "1m" => 1,
                "5m" => 5,
                "15m" => 15,
                "30m" => 30,
                "1h" => 60,
                "2h" => 120,
                "4h" => 240,
                "6h" => 360,
                "8h" => 480,
                "12h" => 720,
                "1d" => 1440,
                _ => 240
            };
        }
    }
}
