using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public interface IBuyManager
    {
        Task Buy(string market, float budget);
    }
}
