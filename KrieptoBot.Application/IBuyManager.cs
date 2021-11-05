using System.Threading.Tasks;
using KrieptoBot.Model;

namespace KrieptoBot.Application
{
    public interface IBuyManager
    {
        Task Buy(Market market, decimal budget);
    }
}
