﻿using KrieptoBod.Infrastructure.Bitvavo.Bitvavo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KrieptoBod.Infrastructure.Bitvavo.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBitvavoService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BitvavoConfig>(configuration.GetSection("Secrets:BitvavoConfig"));

            services.AddTransient<BitvavoAuthHeaderHandler>();

            services.AddRefitClient<IBitvavoApi>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient((serviceProvider, configureClient) =>
                {
                    var bitvavoConfigOptions = serviceProvider.GetService<IOptions<BitvavoConfig>>();
                    var bitvavoConfig = bitvavoConfigOptions.Value;

                    configureClient.BaseAddress = new Uri(bitvavoConfig.BaseUrl);
                    configureClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddHttpMessageHandler<BitvavoAuthHeaderHandler>();

            return services;
        }
    }
}