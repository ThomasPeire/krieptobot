using KrieptoBod.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBod.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBod.ConsoleLauncher
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices();
            services.AddBitvavoService(Configuration);
        }
    }
}