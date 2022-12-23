// LFInteractive LLC. - All Rights Reserved

using FFNodes.Core.Utilities;

namespace FFNodes.Client.Service;

public class Worker : BackgroundService
{
    private readonly ILogger log;

    public Worker()
    {
        log = Instance.log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AdvancedTimer timer = new(TimeSpan.FromSeconds(10))
        {
            Interuptable = true,
            AutoReset = true
        };
        timer.Elapsed += (s, e) =>
        {
            log.Information("Worker running at: {time}", DateTime.Now);
        };
        timer.Start();
        int index = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            log.Debug("Time until Timer elapses: {time}", timer);
            await Task.Delay(1000, stoppingToken);
            index++;
        }
    }
}