using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrieptoBot.DataCollector
{
    public interface ICollector
    {
        Task CollectCandles(IEnumerable<string> markets, ICollection<string> intervals, DateTime fromDateTime,
            DateTime toDateTime);
    }
}