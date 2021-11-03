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
                .AddHeader("6289", "Recommendations!!", "")
                .AddText(message)
                .Show();
        }
    }
}
