using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application
{
    public interface ISellManager
    {
        Task Sell(Market market);
    }
}
