using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Model;

namespace KrieptoBot.DataCollector
{
    public class Collector : ICollector
    {
        private readonly IExchangeService _exchangeService;

        public Collector(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        public async Task CollectCandles(ICollection<string> markets, ICollection<string> intervals, DateTime fromDateTime, DateTime toDateTime)
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
                        currentStartDateTime =
                            currentStartDateTime.AddMinutes(numberOfCandlesInOneCall * intervalInMinutes);

                        var time = currentStartDateTime;
                        tasks.Add(Task.Run(() => _exchangeService.GetCandlesAsync(market, interval, numberOfCandlesInOneCall,
                            time, toDateTime)));
                    }

                    var t = await Task.WhenAll(tasks);

                    var result = new List<Candle>();
                    foreach (var task in tasks)
                    {
                        result.AddRange(task.Result);
                    }

                    Debug.WriteLine(result.Count);
                }
            }

        }

        private int GetIntervalInMinutes(string interval)
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
                _ => 0
            };
        }
    }
}
