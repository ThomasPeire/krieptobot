using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace KrieptoBot.Application;

public interface IDateTimeProvider
{
    Task<DateTime> UtcDateTimeNowSyncedWithExchange();
    Task<DateTime> UtcDateTimeNowExchange();
}

public class DateTimeProvider(
    IExchangeService exchangeService,
    IMemoryCache memoryCache)
    : IDateTimeProvider
{
    private async Task<double> TimeDifferenceWithExchangeInMilliSeconds()
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(TimeDifferenceWithExchangeInMilliSeconds)}",
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

                var utcDateTimeNowExchange = await UtcDateTimeNowExchange();
                var utcDateTimeNow = DateTime.UtcNow;
                return (utcDateTimeNowExchange - utcDateTimeNow).TotalMilliseconds;
            });
    }

    public async Task<DateTime> UtcDateTimeNowSyncedWithExchange() =>
        DateTime.UtcNow.AddMilliseconds(await TimeDifferenceWithExchangeInMilliSeconds());

    public async Task<DateTime> UtcDateTimeNowExchange() => await exchangeService.GetTime();
}