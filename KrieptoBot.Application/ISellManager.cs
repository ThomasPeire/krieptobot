using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application
{
    public interface ISellManager
    {
        Task Sell(Market market);
    }
}