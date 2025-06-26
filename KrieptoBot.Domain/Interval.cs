using System.Diagnostics;

namespace KrieptoBot.Domain;

public class Interval
{
    public string Value { get; set; }
    public static Interval OneMinute => Of(Minutes.One);
    public static Interval FiveMinutes => Of(Minutes.Five);
    public static Interval FifteenMinutes => Of(Minutes.Fifteen);
    public static Interval ThirtyMinutes => Of(Minutes.Thirty);
    public static Interval OneHour => Of(Hours.One);
    public static Interval TwoHours => Of(Hours.Two);
    public static Interval FourHours => Of(Hours.Four);
    public static Interval SixHours => Of(Hours.Six);
    public static Interval EightHours => Of(Hours.Eight);
    public static Interval TwelveHours => Of(Hours.Twelve);
    public static Interval OneDay => Of(Days.One);

    public static Interval Of(string interval)
    {
        return new Interval(interval);
    }

    private Interval(string value)
    {
        Value = value;
    }

    public static implicit operator string(Interval interval)
    {
        return interval.Value;
    }

    public int InMinutes()
    {
        return Value switch
        {
            Minutes.One => 1,
            Minutes.Five => 5,
            Minutes.Fifteen => 15,
            Minutes.Thirty => 30,
            Hours.One => 60,
            Hours.Two => 120,
            Hours.Four => 240,
            Hours.Six => 360,
            Hours.Eight => 480,
            Hours.Twelve => 720,
            Days.One => 1440,
            _ => throw new UnreachableException()
        };
    }

    public static class Minutes
    {
        public const string One = "1m";
        public const string Five = "5m";
        public const string Fifteen = "15m";
        public const string Thirty = "30m";
    }

    static class Hours
    {
        public const string One = "1h";
        public const string Two = "2h";
        public const string Four = "4h";
        public const string Six = "6h";
        public const string Eight = "8h";
        public const string Twelve = "12h";
    }

    static class Days
    {
        public const string One = "1d";
    }
}