using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace KrieptoBot.ConsoleLauncher;

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