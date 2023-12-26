using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KrieptoBot.ConsoleLauncher
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var hostBuilder = HostBuilderWrapper.BuildHost();
                Log.Debug("Starting up");
                await hostBuilder.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
