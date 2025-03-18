using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radius.TestClient;
using RadiusCore.Packet;
using RadiusCore.RadiusConstants;
using System.Net;


var builder = Host.CreateApplicationBuilder();

builder.Services.AddRadiusCore();
builder.Services.AddTransient<RadiusClient>();

var app = builder.Build();

//app.Run();

using var scope = app.Services.CreateScope();

using var client = scope.ServiceProvider.GetRequiredService<RadiusClient>();



var requestPacket = new RadiusPacket(PacketCode.AccessRequest, 0);
requestPacket.AddMessageAuthenticator(); // Add message authenticator for blast radius
requestPacket.AddAttribute("User-Name", "nemo");
requestPacket.AddAttribute("User-Password", "arctangent");

Console.ReadKey();

var responsePacket = await client.SendPacketAsync(
    requestPacket,
    new IPEndPoint(IPAddress.Loopback, 1812));