using KrieptoBot.Application;
using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.Application.Settings;
using KrieptoBot.AzureFunction;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WireMock.Server;

namespace KrieptoBot.Tests.Integration
{
    public class IntegrationTestBase
    {
        private TestServerFactory _serverFactory;
        protected TestServer TestServer { get; private set; }

        [OneTimeSetUp]
        public void BaseOneTimeSetup()
        {
            _serverFactory = new TestServerFactory();
            TestServer = CreateTestServer();
        }

        [OneTimeTearDown]
        public void BaseOneTimeTearDown()
        {
            _serverFactory.Dispose();
        }

        [TearDown]
        public void BaseTearDown()
        {
            TestServer.Services.GetRequiredService<WireMockServer>().Reset();
        }

        private TestServer CreateTestServer()
        {
            return _serverFactory.Create(
                services =>
                {
                    services.AddApplicationServices();
                    services.AddBitvavoService();
                    services.AddScoped<INotificationManager, NotificationManager>();
                }, _ => { });
        }
    }
}
