using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.AzureFunction
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public async static Task Main()
        {
            await HostBuilderWrapper.BuildHost().RunAsync();
        }
    }
}
