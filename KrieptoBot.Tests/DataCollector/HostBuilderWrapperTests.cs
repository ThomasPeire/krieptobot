using KrieptoBot.DataCollector;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace KrieptoBot.Tests.DataCollector
{
    public class HostBuilderWrapperTests

    {
        [Test]
        public void HostBuilderWrapper_ShouldBuildHost()
        {
            var iHost = HostBuilderWrapper.BuildHost();

            Assert.That(iHost.Services.GetRequiredService<ICollector>(), Is.InstanceOf<ICollector>());
        }
    }
}
