using KrieptoBod.Exchange.Bitvavo;
using KrieptoBod.Infrastructure.Exchange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http.Headers;

namespace KrieptoBod.AzureFunction
{
    public static class BitvavoServiceCollectionExtension
    {

        public static IServiceCollection AddBitvavoService(this IServiceCollection services, IConfiguration config)
        {
            var bitvavoApiConfig = config.GetSection("Secrets:BitvavoConfig").Get<BitvavoConfig>();

            services.AddSingleton(bitvavoApiConfig);

            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddHttpClient<BitvavoClient>();

            var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            services.AddTransient<BitvavoAuthHeaderHandler>();
            services.AddRefitClient<IBitvavoApi>(settings)
                .ConfigureHttpClient(x =>
                {
                    x.BaseAddress = new Uri(bitvavoApiConfig.BaseUrl);
                    x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddHttpMessageHandler<BitvavoAuthHeaderHandler>();

            return services;
        }
    }
}
