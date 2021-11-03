using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public interface INotificationManager
    {
        Task SendNotification(string message);
    }
}
