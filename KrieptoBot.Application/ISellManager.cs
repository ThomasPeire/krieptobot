using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public interface ISellManager
    {
        Task Sell(string market);
    }
}
