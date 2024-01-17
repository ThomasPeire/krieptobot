using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KrieptoBot.DataVisualizer
{
    public static class HostBuilderWrapper
    {
        public static IHost BuildHost()
        {
            return CreateHostBuilder().Build();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                    AddAppConfiguration(builder)
                )
                .ConfigureServices(AddServices).UseSerilog();
        }

        private static void AddServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostContext.Configuration)
                .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();

            services
                .AddScoped<ICandlesVisualizer, CandlesVisualizer>()
                .AddHostedService<VisualizationService>();
        }

        private static void AddAppConfiguration(IConfigurationBuilder builder)
        {
            builder.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<CandlesVisualizer>(optional: true)
                .AddEnvironmentVariables();
        }
    }
}
