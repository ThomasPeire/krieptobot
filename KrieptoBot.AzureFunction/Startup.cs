using System;
using System.IO;
using KrieptoBot.Application.Extensions.Microsoft.DependencyInjection;
using KrieptoBot.AzureFunction;
using KrieptoBot.Infrastructure.Bitvavo.Extensions.Microsoft.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace KrieptoBot.AzureFunction
{
    public class Startup: FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.AddApplicationServices();
            builder.Services.AddBitvavoService();
        }
    }
}
