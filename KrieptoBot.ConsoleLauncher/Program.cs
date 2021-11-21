﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KrieptoBot.ConsoleLauncher
{
    internal class Program
    {
        private static async Task Main(string[] args)
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
}
