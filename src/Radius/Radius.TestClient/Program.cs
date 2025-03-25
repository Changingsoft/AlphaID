using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radius.TestClient;
using RadiusCore;
using System.Net;


var builder = Host.CreateApplicationBuilder();

builder.Services.AddRadiusCore();
builder.Services.AddTransient<RadiusClient>();

var app = builder.Build();

//app.Run();

using var scope = app.Services.CreateScope();

using var client = scope.ServiceProvider.GetRequiredService<RadiusClient>();



var requestPacket = new RadiusRequest(PacketCode.AccessRequest, 0, new byte[16], [], new(IPAddress.Loopback, 1812));
//requestPacket.AddMessageAuthenticator(); // Add message authenticator for blast radius
//requestPacket.AddAttribute("User-Name", "nemo");
//requestPacket.AddAttribute("User-Password", "arctangent");

Console.ReadKey();

var responsePacket = await client.SendPacketAsync(
    requestPacket,
    new IPEndPoint(IPAddress.Loopback, 1812), TimeSpan.FromSeconds(3));