using System;

namespace KrieptoBot.Application
{
    public interface IDateTimeProvider
    {
        DateTime UtcDateTimeNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcDateTimeNow() => DateTime.UtcNow;
    }
}
