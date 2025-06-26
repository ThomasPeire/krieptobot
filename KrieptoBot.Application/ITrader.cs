using System.Threading;
using System.Threading.Tasks;

namespace KrieptoBot.Application;

public interface ITrader
{
    Task Run(CancellationToken cancellationToken = default);
}