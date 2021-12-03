using System.Threading.Tasks;
using KrieptoBot.AzureFunction;
using NUnit.Framework;

namespace KrieptoBot.Tests.AzureFunction
{
    public class NotificationManagerTests
    {
        [Test]
        public void SendNotification_Should_Return_CompletedTask()
        {
            var notificationManager = new NotificationManager();
            var result1 = notificationManager.SendNotification("Message");
            var result2 = notificationManager.SendNotification("Message", "SubMessage");

            Assert.That(result1, Is.EqualTo(Task.CompletedTask));
            Assert.That(result2, Is.EqualTo(Task.CompletedTask));
        }
    }
}
