using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KrieptoBot.ConsoleLauncher
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var hostBuilder = CreateHostBuilder(args).Build();
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

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder().ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                new Startup()
                    .ConfigureServices(services);

                services.AddHostedService<TradeService>();
            }).UseSerilog();
    }
}
