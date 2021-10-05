using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.StrategyTester
{
    public class StrategyTesterService : IHostedService
    {

        public StrategyTesterService()
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            /*
             voorbeeld
                interval = 5m
                periode = 1/1/2020 - 31/12/2021
                balance = 1000 euro

                foreach 5m(=currentTime) in periode
                    => trader.run() => 
                        -make sure it uses currentTime 
                        -should talk to historic data fake "api"
                        
                
                return profit/loss
                return overview of trades, recommendations, ...
                
                => make this repeatable for a matrix of configurations
             
             */


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}