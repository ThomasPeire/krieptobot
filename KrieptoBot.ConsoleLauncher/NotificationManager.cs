﻿using System.Threading.Tasks;
using KrieptoBot.Application;

// using Microsoft.Toolkit.Uwp.Notifications;

namespace KrieptoBot.ConsoleLauncher;

public class NotificationManager : INotificationManager
{
    public async Task SendNotification(string message)
    {
        // new ToastContentBuilder()
        //     .AddHeader($"{DateTime.UtcNow:g}", "KrieptoBot!", "")
        //     .AddText(message)
        //     .Show();

        await Task.CompletedTask;
    }

    public async Task SendNotification(string message, string subMessage)
    {
        // new ToastContentBuilder()
        //     .AddHeader($"{DateTime.UtcNow:g}", "KrieptoBot!", "")
        //     .AddText(message)
        //     .AddText(subMessage)
        //     .Show();

        await Task.CompletedTask;
    }
}