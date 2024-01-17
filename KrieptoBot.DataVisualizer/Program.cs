using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KrieptoBot.DataVisualizer
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                var hostBuilder = HostBuilderWrapper.BuildHost();
                Log.Information("Starting up");
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
