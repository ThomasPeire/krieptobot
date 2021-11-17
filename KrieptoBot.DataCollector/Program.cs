using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KrieptoBot.DataCollector
{
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                var hostBuilder = CreateHostBuilder().Build();
                Log.Information("Starting up");
                await hostBuilder.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder().ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppContext.BaseDirectory)
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddUserSecrets<Startup>()
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

                    new Startup().ConfigureServices(services);

                    services.AddHostedService<CollectorService>();
                }).UseSerilog();
        }
    }
}