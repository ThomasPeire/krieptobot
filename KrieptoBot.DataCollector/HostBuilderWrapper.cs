using System;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KrieptoBot.DataCollector
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
                .ConfigureAppConfiguration((context, builder) =>
                    AddAppConfiguration(builder, context)
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

            services.AddScoped<ICollector, Collector>();
            services.AddBitvavoService();

            services.AddHostedService<CollectorService>();
        }

        private static void AddAppConfiguration(IConfigurationBuilder builder, HostBuilderContext context)
        {
            builder.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<Program>(optional: true)
                .AddEnvironmentVariables();
        }
    }
}
