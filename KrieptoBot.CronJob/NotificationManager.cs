using System.Threading.Tasks;
using KrieptoBot.Application;

namespace KrieptoBot.CronJob;

public class NotificationManager : INotificationManager
{
    public Task SendNotification(string message)
    {
        return Task.CompletedTask;
    }

    public Task SendNotification(string message, string subMessage)
    {
        return Task.CompletedTask;
    }
}