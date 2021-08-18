using System;

namespace KrieptoBod.Exchange.Bitvavo.Model
{
    public class CandleDto
    {
        public DateTime TimeStamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
