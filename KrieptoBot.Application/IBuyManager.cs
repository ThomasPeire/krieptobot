using System.Threading.Tasks;
using KrieptoBot.Domain.Trading.ValueObjects;

namespace KrieptoBot.Application
{
    public interface IBuyManager
    {
        Task Buy(Market market, decimal budget);
    }
}