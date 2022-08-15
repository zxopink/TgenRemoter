using TgenRemoteServer;

namespace Company.WebApplication1
{
    public class Program
    {
        //Compile command: dotnet build --configuration Release --runtime ubuntu.20.04-x64
        //publish project command: dotnet publish -c release -r ubuntu.20.04-x64 --self-contained
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}