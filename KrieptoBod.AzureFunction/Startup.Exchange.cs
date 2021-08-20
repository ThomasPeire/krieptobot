using KrieptoBod.Exchange.Bitvavo;
using KrieptoBod.Infrastructure.Exchange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace KrieptoBod.AzureFunction
{
    public static class BitvavoServiceCollectionExtension
    {

        public static IServiceCollection AddBitvavoService(this IServiceCollection services, IConfiguration config)
        {
            var bitvavoApiConfig = config.GetSection("Secrets:BitvavoConfig").Get<BitvavoConfig>();

            services.AddSingleton(bitvavoApiConfig);

            services.AddScoped<IExchangeService, ExchangeService>();
            //services.AddHttpClient<BitvavoClient>();

            services.AddRefitClient<IBitvavoApi>()
                .ConfigureHttpClient(x =>
                {
                    var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    const string httpMethod = "GET";
                    const string body = "";

                    var toHash = timeStamp + httpMethod + bitvavoApiConfig.BaseUrl + body;
                    var signature = GenerateHeaderSignature(toHash, bitvavoApiConfig.ApiSecret);

                    x.BaseAddress = new Uri(bitvavoApiConfig.BaseUrl);
                    x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    x.DefaultRequestHeaders.Add("Bitvavo-Access-Key", bitvavoApiConfig.ApiKey);
                    x.DefaultRequestHeaders.Add("Bitvavo-Access-Window", "20000");
                    x.DefaultRequestHeaders.Add("Bitvavo-Access-Timestamp", timeStamp);
                    x.DefaultRequestHeaders.Add("Bitvavo-Access-Signature", signature);
                });

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

        private static string GenerateHeaderSignature(string toHash, string apiSecret)
        {
            var encoding = new UTF8Encoding();

            var textBytes = encoding.GetBytes(toHash);
            var keyBytes = encoding.GetBytes(apiSecret);

            byte[] hashBytes;

            using (var hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
