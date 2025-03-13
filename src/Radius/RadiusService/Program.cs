using RadiusCore;
using RadiusCore.Dictionary;
using RadiusCore.Packet;
using RadiusServer;
using RadiusService;
using System.Net;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "RadiusService";
});
builder.Services.AddHostedService<Worker>();

builder.Services.AddTransient<IRadiusDictionary>(services =>
{
    return RadiusDictionary.LoadAsync(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\content\\radius.dictionary").GetAwaiter().GetResult();
});
builder.Services.AddTransient<RadiusPacketParser>();
builder.Services.AddTransient<TestPacketHandler>();
builder.Services.AddTransient<UdpClientFactory>();
builder.Services.AddTransient<PacketHandlerRepository>(services =>
{
    var packetHandler = services.GetRequiredService<TestPacketHandler>();
    var repository = new PacketHandlerRepository();
    repository.AddPacketHandler(IPAddress.Parse("127.0.0.1"), packetHandler, "secret");
    return repository;
});
builder.Services.AddTransient<RadiusServer.RadiusServer>(services =>
{
    var udpClientFactory = services.GetRequiredService<UdpClientFactory>();
    var radiusPacketParser = services.GetRequiredService<RadiusPacketParser>();
    var repository = services.GetRequiredService<PacketHandlerRepository>();
    return new RadiusServer.RadiusServer(
        udpClientFactory,
        new IPEndPoint(IPAddress.Any, 1812),
        radiusPacketParser,
        RadiusServerType.Authentication,
        repository,
        services.GetRequiredService<ILogger<RadiusServer.RadiusServer>>());
});

var host = builder.Build();
host.Run();
