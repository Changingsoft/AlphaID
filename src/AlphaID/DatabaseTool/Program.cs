using DatabaseTool;
using DatabaseTool.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

var platform = builder.Services.AddAlphaIdPlatform();
platform.AddEntityFramework(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql =>
        {
            sql.UseNetTopologySuite();
            sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
        });
});

//用于IdentityServer的ConfigurationDbContext和PersistenceGrantDbContext
builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
    });



//数据库执行器
builder.Services.AddScoped<DatabaseExecutor>().Configure<DatabaseExecutorOptions>(builder.Configuration.GetSection("DatabaseExecutorOptions"));

//配置迁移器
builder.Services.AddScoped<DatabaseMigrator, IdServerConfigurationDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, IdServerPersistedGrantDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, IdSubjectsDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, RealNameDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, DirectoryLogonDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, AdminCenterDbMigrator>();
builder.Services.AddScoped<DatabaseMigrator, AuditLogDbMigrator>();

IHost host = builder.Build();

await using AsyncServiceScope scope = host.Services.CreateAsyncScope();
var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
var connectionString = configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(@"即将开始执行数据库工具。您将使用下列选项来处理数据：");
Console.WriteLine($@"- 环境: {environment.EnvironmentName}");
Console.WriteLine($@"- 数据库连接字符串: {connectionString}");
Console.WriteLine($@"- 删除数据库: {scope.ServiceProvider.GetRequiredService<IOptions<DatabaseExecutorOptions>>().Value.DropDatabase}");
Console.WriteLine($@"- 应用迁移: {scope.ServiceProvider.GetRequiredService<IOptions<DatabaseExecutorOptions>>().Value.ApplyMigrations}");
Console.WriteLine($@"- 迁移后处理: {scope.ServiceProvider.GetRequiredService<IOptions<DatabaseExecutorOptions>>().Value.ExecutePostMigrations}");
Console.WriteLine($@"- 添加测试数据: {scope.ServiceProvider.GetRequiredService<IOptions<DatabaseExecutorOptions>>().Value.AddTestingData}");
if (!args.Contains("NonInteractive", StringComparer.OrdinalIgnoreCase))
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

var executor = scope.ServiceProvider.GetRequiredService<DatabaseExecutor>();
await executor.ExecuteAsync();

if (!args.Contains("NonInteractive", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine(@"数据库工具已执行完成。按任意键退出...");
    Console.ReadKey(true);
}
