using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace KrieptoBot.DataCollector
{
    class Program
    {
        static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder().ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppContext.BaseDirectory)
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets<Program>()
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var startup = new Startup(hostContext.Configuration);

                    startup.ConfigureServices(services);

                    services.AddHostedService<CollectorService>();
                });
    }
}
