using Flexinets.Radius.Core;
using Microsoft.Extensions.Logging;
using System.Net;
using Flexinets.Radius;

namespace RadiusService;

public class Worker : BackgroundService
{
        private RadiusServer _authenticationServer;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var dictionary = await RadiusDictionary.LoadAsync(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\content\\radius.dictionary");
        var radiusPacketParser = new RadiusPacketParser(null, dictionary);
        var packetHandler = new TestPacketHandler();
        var repository = new PacketHandlerRepository();

        repository.AddPacketHandler(IPAddress.Parse("127.0.0.1"), packetHandler, "secret");

        _authenticationServer = new RadiusServer(
            new Flexinets.Net.UdpClientFactory(),
            new IPEndPoint(IPAddress.Any, 1812),
            radiusPacketParser,
            RadiusServerType.Authentication,
            repository,
            null);
        _authenticationServer.Start();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _authenticationServer?.Stop();
        _authenticationServer?.Dispose(); 
        return base.StopAsync(cancellationToken);
    }
}
