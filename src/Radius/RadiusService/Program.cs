

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "RadiusService";
});

builder.Services.AddRadiusServer();

var host = builder.Build();
host.Run();
