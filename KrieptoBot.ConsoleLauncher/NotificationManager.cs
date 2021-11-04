using System;
using System.Threading.Tasks;
using KrieptoBot.Application;
using Microsoft.Toolkit.Uwp.Notifications;

namespace KrieptoBot.ConsoleLauncher
{
    public class NotificationManager : INotificationManager
    {
        public async Task SendNotification(string message)
        {
            new ToastContentBuilder()
                .AddHeader($"{DateTime.UtcNow:g}", "Recommendations!", "")
                .AddText(message)
                .Show();
        }

        public async Task SendNotification(string message, string subMessage)
        {
            new ToastContentBuilder()
                .AddHeader($"{DateTime.UtcNow:g}", "Recommendations!", "")
                .AddText(message)
                .AddText(subMessage)
                .Show();
        }

        public async Task SendNotification(string message, string subMessage, string subSubMessage)
        {
            new ToastContentBuilder()
                .AddHeader($"{DateTime.UtcNow:g}", "Recommendations!", "")
                .AddText(message)
                .AddText(subMessage)
                .AddText(subSubMessage)
                .Show();
        }
    }
}
