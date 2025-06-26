using System;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper;

public static class HelperMethods
{
    public static long? ToUnixTimeMilliseconds(this DateTime? dateTime)
    {
        return dateTime != null ? ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds() : null;
    }

    public static decimal RoundToSignificantDigits(this decimal d, int digits,
        MidpointRounding roundingMode = MidpointRounding.AwayFromZero)
    {
        if (d == 0)
            return 0;

        var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs((double)d))) + 1);
        return (decimal)(scale * Math.Round((double)d / scale, digits, roundingMode));
    }
}