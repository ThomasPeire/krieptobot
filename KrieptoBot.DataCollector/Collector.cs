using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using KrieptoBot.Application;
using KrieptoBot.Domain;
using KrieptoBot.Domain.Trading.Entity;
using KrieptoBot.Domain.Trading.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KrieptoBot.DataCollector
{
    public class Collector : ICollector
    {
        private static readonly SemaphoreSlim Semaphore = new(10);
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<Collector> _logger;
        
        public Collector(IExchangeService exchangeService, ILogger<Collector> logger)
        {
            _exchangeService = exchangeService;
            _logger = logger;
        }

        public async Task CollectCandles(IEnumerable<string> markets, ICollection<string> intervals,
            DateTime fromDateTime, DateTime toDateTime, CancellationToken ct)
        {
            foreach (var market in markets)
            {
                foreach (var interval in intervals)
                {
                    var tasks = GetDownloadTasks(fromDateTime, toDateTime, interval, market, ct);

                    await Task.WhenAll(tasks);

                    var candles = new List<Candle>();
                    foreach (var task in tasks) candles.AddRange(await task);

                    var json = JsonSerializer.Serialize(candles.OrderBy(x => x.TimeStamp));
                    await File.WriteAllTextAsync($@"D:\{market}-{interval}.json", json, ct);
                }
            }
        }
        
        private List<Task<IEnumerable<Candle>>> GetDownloadTasks(DateTime fromDateTime, DateTime toDateTime, string interval, string market, CancellationToken ct)
        {
            var tasks = new List<Task<IEnumerable<Candle>>>();
            var intervalInMinutes = Interval.Of(interval).InMinutes();

            var currentStartDateTime = fromDateTime;
            const int numberOfCandlesInOneCall = 1000;

            while (currentStartDateTime <= toDateTime)
            {
                tasks.Add(
                    GetDownloadTask(toDateTime, currentStartDateTime, intervalInMinutes, numberOfCandlesInOneCall, market, interval, ct));

                currentStartDateTime =
                    currentStartDateTime.AddMinutes(numberOfCandlesInOneCall * intervalInMinutes);
            }

            return tasks;
        }
        
        private Task<IEnumerable<Candle>> GetDownloadTask(DateTime toDateTime, DateTime currentStartDateTime, int intervalInMinutes, int numberOfCandlesInOneCall, string market, string interval,CancellationToken ct)
        {
            var startTime = currentStartDateTime;
            var endTime =
                new DateTime(Math.Min(
                    currentStartDateTime.AddMinutes(intervalInMinutes * numberOfCandlesInOneCall).Ticks,
                    toDateTime.Ticks));
            var task = CreateTask(numberOfCandlesInOneCall, market, interval, startTime, endTime, ct);
            return task;
        }

        private async Task<IEnumerable<Candle>> CreateTask(int numberOfCandlesInOneCall, string market, string interval, DateTime startTime, DateTime endTime, CancellationToken ct)
        {
            
            await Semaphore.WaitAsync(ct);
            
            ct.ThrowIfCancellationRequested();
            try
            {
                _logger.LogInformation("Downloading {Interval} candles for {Market} from {StartTime} to {EndTime}", market, interval, startTime, endTime);
                return await _exchangeService.GetCandlesAsync(market, interval, numberOfCandlesInOneCall,
                    startTime, endTime, ct);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
