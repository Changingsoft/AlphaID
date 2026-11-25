using AlphaId.DatabasePreparation;
using DatabasePreparation;
using DatabaseTool;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("未配置连接字符串 ConnectionStrings:DefaultConnection。");

//真正的建库逻辑（上下文装配、迁移、内置数据、样例数据）全在 DatabasePreparation 里，
//与集成测试共用同一份实现。本文件只负责：读配置 → 打印 → 确认 → 调用。
builder.Services.AddAlphaIdDatabasePreparation(connectionString);

//先绑旧节、再绑新节：同一个键上后者覆盖前者，于是新节优先，旧部署脚本不必立刻改。
builder.Services.Configure<DatabasePrepareOptions>(builder.Configuration.GetSection(DatabaseToolOptions.LegacySectionName));
builder.Services.Configure<DatabasePrepareOptions>(builder.Configuration.GetSection(DatabaseToolOptions.SectionName));

IHost host = builder.Build();

await using AsyncServiceScope scope = host.Services.CreateAsyncScope();
IServiceProvider services = scope.ServiceProvider;

IHostEnvironment environment = services.GetRequiredService<IHostEnvironment>();
DatabasePrepareOptions options = services.GetRequiredService<IOptions<DatabasePrepareOptions>>().Value;

Console.WriteLine(@"即将开始执行数据库工具。您将使用下列选项来处理数据：");
Console.WriteLine($@"- 环境: {environment.EnvironmentName}");
Console.WriteLine($@"- 数据库连接字符串: {connectionString}");
Console.WriteLine($@"- 删除数据库: {options.DropDatabase}");
Console.WriteLine($@"- 应用迁移: {options.ApplyMigrations}");
Console.WriteLine($@"- 添加初始化数据: {options.AddInitData}");
Console.WriteLine($@"- 强制对齐内置数据: {options.OverwriteInitData}");
Console.WriteLine($@"- 添加测试数据: {options.AddTestingData}");

bool interactive = !args.Contains("NonInteractive", StringComparer.OrdinalIgnoreCase);
if (interactive)
{
    Console.WriteLine(@"此操作不可逆，请务必核实设置是否正确，并在操作前备份数据库。是否继续（y/N）");
    if (Console.ReadKey(true).Key != ConsoleKey.Y)
    {
        Console.WriteLine(@"用户取消了操作。按任意键退出...");
        Console.ReadKey(true);
        return;
    }
}

Console.WriteLine(@"数据库工具开始执行...");

DatabasePreparationExecutor executor = services.GetRequiredService<DatabasePreparationExecutor>();
await executor.ExecuteAsync(options);

if (interactive)
{
    Console.WriteLine(@"数据库工具已执行完成。按任意键退出...");
    Console.ReadKey(true);
}
