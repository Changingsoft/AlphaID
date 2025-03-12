using System.Net;
using Microsoft.Extensions.Logging;
using Radius.TestServer;
using Radius.TestServer.PacketHandler;
using RadiusCore.Dictionary;
using RadiusCore.Packet;
using UdpClient;

var loggerFactory = LoggerFactory.Create(o =>
{
    o.AddSimpleConsole(c => c.SingleLine = true);
    o.SetMinimumLevel(LogLevel.Trace);
});

var handlerRepository = new PacketHandlerRepository();
handlerRepository.AddPacketHandler(IPAddress.Any, new TestPacketHandler(), "xyzzy5461");

var server = new RadiusServer(
    new UdpClientFactory(),
    new IPEndPoint(IPAddress.Any, 1812),
    new RadiusPacketParser(
        loggerFactory.CreateLogger<RadiusPacketParser>(),
        RadiusDictionary.Parse(DefaultDictionary.RadiusDictionary)),
    RadiusServerType.Authentication,
    handlerRepository,
    loggerFactory.CreateLogger<RadiusServer>());


server.Start();

Console.WriteLine("Press any key to stop...");
Console.ReadKey();
Console.WriteLine("Stopping...");
server.Stop();