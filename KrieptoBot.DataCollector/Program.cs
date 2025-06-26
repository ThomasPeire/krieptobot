using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KrieptoBot.DataCollector;

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
            Log.CloseAndFlush();
        }
    }
}