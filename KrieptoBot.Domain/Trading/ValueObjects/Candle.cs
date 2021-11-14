using System;

namespace KrieptoBot.Domain.Trading.ValueObjects
{
    public class Candle
    {
        private Candle()
        {
        }

        public Candle(DateTime timeStamp, Price high, Price low, Price open, Price close, decimal volume)
        {
            ArgumentNullException.ThrowIfNull(timeStamp);

            if (volume < 0m)
                throw new ArgumentException("Volume can not be negative", nameof(volume));

            TimeStamp = timeStamp;
            High = high;
            Low = low;
            Open = open;
            Close = close;
            Volume = volume;
        }

        public DateTime TimeStamp { get; }

        public Price High { get; }

        public Price Low { get; }

        public Price Open { get; }

        public Price Close { get; }

        public decimal Volume { get; }
    }
}