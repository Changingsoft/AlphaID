using RadiusCore;

namespace RadiusService;

public class Worker(RadiusServer radiusServer) : BackgroundService
{

    public override async Task StartAsync(CancellationToken cancellationToken)
    {

        await radiusServer.Start();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        radiusServer.Stop();
        radiusServer.Dispose();
        return Task.CompletedTask;
    }
}
