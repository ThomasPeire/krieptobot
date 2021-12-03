using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace KrieptoBot.AzureFunction
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main()
        {
            HostBuilderWrapper.BuildHost().Run();
        }
    }
}
