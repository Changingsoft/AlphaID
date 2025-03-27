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


Console.ReadKey();

