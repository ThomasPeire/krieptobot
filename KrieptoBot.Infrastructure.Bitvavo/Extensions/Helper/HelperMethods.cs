using System;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions.Helper
{
    public static class HelperMethods
    {
        public static long? ToUnixTimeMilliseconds(this DateTime? dateTime)
        {
            return dateTime != null ? ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds() : null;
        }

        public static int GetIntervalInMinutes(string interval)
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


        public static decimal RoundToSignificantDigits(this decimal d, int digits){
            if(d == 0)
                return 0;

            var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs((double)d))) + 1);
            return (decimal)(scale * Math.Round((double)d / scale, digits, MidpointRounding.AwayFromZero));
        }
    }
}
