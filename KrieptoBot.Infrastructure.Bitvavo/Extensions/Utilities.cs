using System;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions;

public static class Utilities
{
    public static decimal TruncateToSignificantDigits(this decimal d, int digits){
        if(d == 0)
            return 0;

        var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs((double)d))) + 1 - digits);
        return (decimal)(scale * Math.Truncate((double)d / scale));
    }
}
