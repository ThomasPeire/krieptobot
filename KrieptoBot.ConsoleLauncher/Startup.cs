using KrieptoBot.Application;
using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.Application.Settings;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBot.ConsoleLauncher
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<RecommendatorSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("RecommendatorSettings").Bind(settings);
                });
            services.AddApplicationServices();
            services.AddBitvavoService();
            services.AddScoped<INotificationManager, NotificationManager>();
        }
    }
}