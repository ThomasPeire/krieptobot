using KrieptoBod.Exchange.Bitvavo;
using KrieptoBod.Infrastructure.Exchange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KrieptoBod.AzureFunction
{
    public static class BitvavoServiceCollectionExtension
    {

        public static IServiceCollection AddBitvavoService(this IServiceCollection services, IConfiguration config)
        {
            var bitvavoApiConfig = new BitvavoConfig();
            config.GetSection("Secrets:BitvavoConfig").Bind(bitvavoApiConfig);

            services.AddSingleton(bitvavoApiConfig);

            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddHttpClient<BitvavoClient>();

            return services;
        }
    }
}
