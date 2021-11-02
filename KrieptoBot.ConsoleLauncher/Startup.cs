using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBot.ConsoleLauncher
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddApplicationServices();
            services.AddBitvavoService();
        }
    }
}
