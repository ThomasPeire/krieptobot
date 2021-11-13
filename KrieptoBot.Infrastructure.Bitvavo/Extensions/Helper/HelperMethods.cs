using System;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper
{
    internal static class HelperMethods
    {
        public static long? ToUnixTimeMilliseconds(this DateTime? dateTime)
        {
            return dateTime != null ? ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds() : null;
        }
    }
}