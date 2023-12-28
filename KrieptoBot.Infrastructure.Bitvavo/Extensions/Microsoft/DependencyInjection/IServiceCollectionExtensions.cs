using System;
using System.Net.Http;
using System.Net.Http.Headers;
using KrieptoBot.Application;
using KrieptoBot.Infrastructure.Bitvavo.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddBitvavoService(this IServiceCollection services)
        {
            services.AddOptions<BitvavoConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Secrets:BitvavoConfig").Bind(settings);
                });
            services.AddScoped<IMemoryCache, MemoryCache>();
            services.AddTransient<BitvavoAuthHeaderHandler>();
            services.AddTransient<BadRequestLoggingHandler>();

            services.AddScoped<IExchangeService, BitvavoService>();
            services.AddRefitClient<IBitvavoApi>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient((serviceProvider, configureClient) =>
                {
                    var bitvavoConfigOptions = serviceProvider.GetService<IOptions<BitvavoConfig>>();
                    if (bitvavoConfigOptions != null)
                    {
                        var bitvavoConfig = bitvavoConfigOptions.Value;

                        configureClient.BaseAddress = new Uri(bitvavoConfig.BaseUrl);
                    }

                    configureClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddHttpMessageHandler<BitvavoAuthHeaderHandler>()
                .AddHttpMessageHandler<BadRequestLoggingHandler>()
                .AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
