using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBot.DataCollector
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICollector, Collector>();
            services.AddBitvavoService();
        }
    }
}
