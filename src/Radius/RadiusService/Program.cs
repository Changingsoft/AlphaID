

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "RadiusService";
});
builder.Services.AddLogging(options =>
{
    options.AddConsole();
});

builder.Services.AddRadiusServer();

var host = builder.Build();
host.Run();
