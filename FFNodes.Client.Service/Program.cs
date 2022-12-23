// LFInteractive LLC. - All Rights Reserved
using Serilog;

namespace FFNodes.Client.Service;

public class Program
{
    public static void Main(string[] args)
    {
        ILogger log = Instance.log;
        log.Information($"Service is Starting!");
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
            })
            .UseSerilog()
            .UseWindowsService()
            .Build();

        host.Run();
    }
}