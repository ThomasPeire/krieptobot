using KrieptoBot.Application;
using KrieptoBot.Application.Settings;
using KrieptoBot.AzureFunction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace KrieptoBot.Tests.AzureFunction;

public class HostBuilderWrapperTests

{
    [Test]
    public void HostBuilderWrapper_ShouldBuildHost()
    {
        var iHost = HostBuilderWrapper.BuildHost();

        Assert.That(iHost.Services.GetRequiredService<IOptions<RecommendatorSettings>>(),
            Is.InstanceOf<IOptions<RecommendatorSettings>>());
        Assert.That(iHost.Services.GetRequiredService<IOptions<TradingSettings>>(),
            Is.InstanceOf<IOptions<TradingSettings>>());
        Assert.That(iHost.Services.GetRequiredService<INotificationManager>(), Is.InstanceOf<INotificationManager>());
        Assert.That(iHost.Services.GetRequiredService<ITrader>(), Is.InstanceOf<ITrader>());
    }
}