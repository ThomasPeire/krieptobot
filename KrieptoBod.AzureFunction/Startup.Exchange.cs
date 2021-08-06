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
            services.AddSingleton<IExchangeService>(x =>
            {
                var bitvavoApiDetails =
                    config.GetSection("Secrets:BitvavoApiDetails").Get<Settings.BitvavoApiDetails>();
                return new ExchangeService(bitvavoApiDetails.ApiKey, bitvavoApiDetails.ApiSecret, bitvavoApiDetails.BaseUrl);
            });

            return services;
        }
    }
}
