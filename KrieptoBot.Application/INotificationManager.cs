using System.Threading.Tasks;

namespace KrieptoBot.Application
{
    public interface INotificationManager
    {
        Task SendNotification(string message);
        Task SendNotification(string message, string subMessage);
    }
}