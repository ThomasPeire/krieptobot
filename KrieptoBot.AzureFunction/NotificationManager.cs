using System.Threading.Tasks;
using KrieptoBot.Application;

namespace KrieptoBot.AzureFunction;

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