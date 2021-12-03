using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Server;

namespace KrieptoBot.Tests.Integration
{
    public class TestServerFactory
    {
        private readonly List<TestServer> _instances = new();

        public TestServer Create(
            Action<IServiceCollection> configureServices,
            Action<IApplicationBuilder> configureApplication)
        {
            var wiremockServer = WireMockServer.Start();
            var builder = new WebHostBuilder()
                .Configure(configureApplication)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true);

                    configurationBuilder.AddInMemoryCollection(new KeyValuePair<string, string>[]
                    {
                        new("Secrets:BitvavoConfig:BaseUrl", wiremockServer.Urls[0])
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddHttpContextAccessor();
                    configureServices?.Invoke(services);
                    services.AddSingleton(wiremockServer);
                });

            var server = new TestServer(builder);
            _instances.Add(server);
            return server;
        }

        public void Dispose()
        {
            foreach (var testServer in _instances)
            {
                testServer.Dispose();
            }
        }
    }
}
