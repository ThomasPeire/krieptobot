using Microsoft.Extensions.Hosting;

namespace KrieptoBot.AzureFunction
{
    public static class Program
    {
        public static void Main()
        {
            HostBuilderWrapper.BuildHost().Run();
        }
    }
}
