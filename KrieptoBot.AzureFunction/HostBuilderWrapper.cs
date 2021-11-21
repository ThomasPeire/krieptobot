using System;
using System.IO;
using KrieptoBot.Application;
using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.Application.Settings;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.AzureFunction
{
    public static class HostBuilderWrapper
    {
        public static IHost BuildHost()
        {
            return new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((context, builder) => AddAppConfiguration(builder, context))
                .ConfigureServices(AddServices)
                .Build();
        }

        private static void AddAppConfiguration(IConfigurationBuilder builder, HostBuilderContext context)
        {
            builder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"),
                    true,
                    false)
                .AddUserSecrets<TradeFunction>()
                .AddEnvironmentVariables();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddOptions<RecommendatorSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("RecommendatorSettings").Bind(settings);
                });
            services.AddOptions<TradingSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("TradingSettings").Bind(settings);
                });
            services.AddApplicationServices();
            services.AddBitvavoService();
            services.AddScoped<INotificationManager, NotificationManager>();
        }
    }
}
