using System;
using System.IO;
using KrieptoBot.Application;
using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.AzureFunction
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((context, builder) =>
                    builder
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"),
                            optional: true,
                            reloadOnChange: false)
                        .AddUserSecrets<Program>()
                        .AddEnvironmentVariables())
                .ConfigureServices(services =>
                {
                    services.AddOptions();
                    services.AddApplicationServices();
                    services.AddBitvavoService();
                    services.AddScoped<INotificationManager, NotificationManager>();
                })
                .Build();

            host.Run();
        }
    }
}
