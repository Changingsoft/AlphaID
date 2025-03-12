using System.Net;
using RadiusCore.Dictionary;
using RadiusCore.Packet;
using RadiusServer;
using UdpClient;

namespace RadiusService;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
        private RadiusServer.RadiusServer _authenticationServer;

        public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var dictionary = await RadiusDictionary.LoadAsync(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\content\\radius.dictionary");
        var radiusPacketParser = new RadiusPacketParser(null, dictionary);
        var packetHandler = new TestPacketHandler();
        var repository = new PacketHandlerRepository();

        repository.AddPacketHandler(IPAddress.Parse("127.0.0.1"), packetHandler, "secret");

        _authenticationServer = new RadiusServer.RadiusServer(
            new UdpClientFactory(),
            new IPEndPoint(IPAddress.Any, 1812),
            radiusPacketParser,
            RadiusServerType.Authentication,
            repository,
            null);
        _authenticationServer.Start();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _authenticationServer.Stop();
        _authenticationServer.Dispose(); 
        return base.StopAsync(cancellationToken);
    }
}
