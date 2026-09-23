using AlphaId.EntityFramework;
using AlphaId.EntityFramework.Admin;
using AlphaId.EntityFramework.DirectoryAccountManagement;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.EntityFramework.RealName;
using AlphaId.EntityFramework.SecurityAuditing;
using AlphaId.InitData;
using AlphaId.Migrations;
using AlphaId.TestingData;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace IntegrationTestUtilities;

/// <summary>
/// 由集成测试自己准备并拥有的实体数据库。
/// </summary>
/// <remarks>
/// <para>
/// 在此之前的集成测试直接连 <c>appsettings.Development.json</c> 里的开发库，于是隐含了两个前提：
/// 「那个库存在」且「有人先用 DatabaseTool 灌过数据」。这两个前提在别的机器、干净的 CI 上都不成立，
/// 而失败方式通常很难看懂（令牌端点 400、组织接口 404，看起来像业务 bug）。
/// </para>
/// <para>
/// 现在改为：每个测试进程建一个**唯一命名**的库，跑完全部 8 套 EF 迁移，再灌内置数据与样例数据，
/// 进程退出时删除。于是测试既不依赖开发库，也不需要 DatabaseTool，并且每次运行都从同一个已知状态出发——
/// 这正是「可回归」的含义。
/// </para>
/// <para>
/// 库名字固定为 <c>AlphaIdTest-{测试程序集}-net{主版本}</c>（而不是每次随机），这样重复运行会**重建**同一个库，
/// 既不会积累垃圾库，也不会被上一次跑崩留下的脏数据影响。想保留现场便于排查时，设置环境变量
/// <see cref="KeepDatabaseVariable"/> 即可跳过退出时的删除。
/// </para>
/// </remarks>
public sealed class TestDatabase
{
    /// <summary>
    /// 覆盖连接字符串模板的环境变量名。
    /// </summary>
    /// <remarks>
    /// 不设置时使用 LocalDB。模板里的 <c>{Database}</c> 会被替换成测试库名；也可以直接给完整连接字符串，
    /// 库名会被强制改写为测试库名。CI 上（例如 Linux 跑 SQL Server 容器）传入
    /// <c>Server=localhost;User Id=sa;Password=...;TrustServerCertificate=True</c> 即可。
    /// </remarks>
    public const string ConnectionStringVariable = "ALPHAID_TEST_CONNECTION_STRING";

    /// <summary>
    /// 设置该环境变量（任意非空值）后，测试进程结束时不会删除测试数据库。
    /// </summary>
    public const string KeepDatabaseVariable = "ALPHAID_TEST_KEEP_DATABASE";

    private const string DefaultConnectionStringTemplate =
        @"Server=(localdb)\MSSQLLocalDB;Database={Database};Trusted_Connection=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

    private static readonly ConcurrentDictionary<string, Lazy<TestDatabase>> Cache = new(StringComparer.OrdinalIgnoreCase);

    private TestDatabase(string name, string connectionString)
    {
        Name = name;
        ConnectionString = connectionString;
    }

    /// <summary>测试数据库的名称。</summary>
    public string Name { get; }

    /// <summary>指向测试数据库的连接字符串。</summary>
    public string ConnectionString { get; }

    /// <summary>
    /// 取得（必要时创建）指定测试程序集对应的测试数据库。
    /// </summary>
    /// <param name="testAssembly">测试程序集，用于区分同一台机器上并行运行的不同测试项目。</param>
    /// <returns>已就绪的测试数据库；同一程序集在同一次进程内只会创建一次。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="testAssembly"/> 为 <see langword="null"/>。</exception>
    /// <remarks>
    /// 这里刻意要求显式传入程序集，而不是用 <see cref="Assembly.GetEntryAssembly"/> 推断：
    /// 在 VSTest 下入口程序集是 <c>testhost</c>，多个测试项目会因此撞到同一个库名。
    /// </remarks>
    public static TestDatabase For(Assembly testAssembly)
    {
        ArgumentNullException.ThrowIfNull(testAssembly);
        string key = $"{testAssembly.GetName().Name}-{FrameworkSuffix()}";
        return Cache.GetOrAdd(
            key,
            _ => new Lazy<TestDatabase>(() => Task.Run(Provision).GetAwaiter().GetResult(), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }

    /// <summary>
    /// 返回需要写进测试宿主配置的连接字符串覆盖项。
    /// </summary>
    /// <returns>配置键到连接字符串的映射，由调用方加到宿主的配置源中。</returns>
    /// <remarks>
    /// 两个应用都从 <c>ConnectionStrings:DefaultConnection</c> 取库；
    /// <c>AdminWebApp</c> 另有一处按 <c>OperationalDbContext</c> 取名的注册，一并覆盖以免留下指向开发库的缺口。
    /// </remarks>
    public IReadOnlyDictionary<string, string?> ConnectionStringOverrides() => new Dictionary<string, string?>(StringComparer.Ordinal)
    {
        ["ConnectionStrings:DefaultConnection"] = ConnectionString,
        ["ConnectionStrings:OperationalDbContext"] = ConnectionString,
    };

    /// <inheritdoc />
    public override string ToString() => $"{Name} ({ConnectionString})";

    private static TestDatabase Provision()
    {
        string name = BuildDatabaseName();
        string connectionString = BuildConnectionString(name);

        // 先删后建：保证每次运行都从同一个干净状态出发。
        DropIfExists(name, connectionString);

        using ServiceProvider provider = BuildServices(connectionString).BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        Migrate(scope.ServiceProvider);
        SeedInitData(scope.ServiceProvider);
        SeedSampleData(scope.ServiceProvider);

        TestDatabase database = new(name, connectionString);
        AppDomain.CurrentDomain.ProcessExit += (_, _) => database.TryDrop();
        return database;
    }

    /// <summary>
    /// 按 DatabaseTool 的方式装配服务，再从容器里取 DbContext 使用。
    /// </summary>
    /// <remarks>
    /// 不自己 <c>new DbContextOptions&lt;T&gt;()</c> 的理由有两个：
    /// 一是 Duende 的 <c>ConfigurationDbContext</c> 会从容器里取 <c>ConfigurationStoreOptions</c>，
    /// 裸造上下文会以 "Unable to resolve service for type ... ConfigurationStoreOptions" 失败；
    /// 二是走同一套装配能保证测试建的库与 <c>DatabaseTool</c> 建的库 schema 完全一致。
    /// </remarks>
    private static ServiceCollection BuildServices(string connectionString)
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddAlphaIdPlatform().AddEntityFramework(options =>
            options.UseSqlServer(connectionString, ConfigureSqlServer));
        services.AddIdentityServer()
            .AddConfigurationStore(options =>
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, ConfigureSqlServer))
            .AddOperationalStore(options =>
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, ConfigureSqlServer));
        return services;
    }

    private static void ConfigureSqlServer(SqlServerDbContextOptionsBuilder sql)
    {
        // 迁移住在 AlphaId.Migrations 里，而各 DbContext 住在别处，必须显式指定迁移程序集，
        // 否则 Migrate() 找不到迁移，只会建出一个空库。
        sql.MigrationsAssembly(MigrationAssembly.Name);
        sql.UseNetTopologySuite();
    }

    /// <summary>应用全部 DbContext 的迁移。它们共用同一个库，各自把迁移记录写进 __EFMigrationsHistory。</summary>
    private static void Migrate(IServiceProvider services)
    {
        services.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
        services.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        services.GetRequiredService<AlphaIdIdentityDbContext>().Database.Migrate();
        services.GetRequiredService<RealNameDbContext>().Database.Migrate();
        services.GetRequiredService<DirectoryLogonDbContext>().Database.Migrate();
        services.GetRequiredService<OperationalDbContext>().Database.Migrate();
        services.GetRequiredService<LoggingDbContext>().Database.Migrate();
        services.GetRequiredService<AlphaIdDbContext>().Database.Migrate();
    }

    /// <summary>
    /// 灌 IdentityServer 内置数据，并顺带证明「刚灌完的库与代码定义完全一致」。
    /// </summary>
    /// <remarks>
    /// 第一次 <c>EnsureAsync</c> 把缺失的行插进去；紧接着再跑一次，此时所有行都已存在，
    /// 必须一行都不插、一条差异告警都没有。若第二次出现告警，说明某个字段经数据库往返后不再是代码里的值
    /// （长度截断、大小写折叠、时间精度丢失之类），这类问题如果放到测试里才暴露会非常难查。
    /// </remarks>
    private static void SeedInitData(IServiceProvider services)
    {
        ConfigurationDbContext db = services.GetRequiredService<ConfigurationDbContext>();
        InitDataResult seed = InitDataSeeder.EnsureAsync(db).GetAwaiter().GetResult();
        if (seed.Inserted == 0)
        {
            throw new InvalidOperationException("新建的测试数据库里应当没有内置数据，但 EnsureAsync 一行都没有插入。");
        }

        InitDataResult verify = InitDataSeeder.EnsureAsync(db).GetAwaiter().GetResult();
        if (verify.Inserted != 0 || verify.Warnings.Count != 0)
        {
            throw new InvalidOperationException(
                $"内置数据经数据库往返后与代码定义不一致：新插入 {verify.Inserted} 行。" +
                Environment.NewLine + string.Join(Environment.NewLine, verify.Warnings));
        }
    }

    private static void SeedSampleData(IServiceProvider services)
    {
        SampleDataSeeder.SeedAsync(services.GetRequiredService<AlphaIdIdentityDbContext>()).GetAwaiter().GetResult();
        SampleDataSeeder.SeedAsync(services.GetRequiredService<AlphaIdDbContext>()).GetAwaiter().GetResult();
        SampleDataSeeder.SeedAsync(services.GetRequiredService<OperationalDbContext>()).GetAwaiter().GetResult();
    }

    private static string BuildDatabaseName()
    {
        string assembly = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
        return $"AlphaIdTest-{Sanitize(assembly)}-{FrameworkSuffix()}";
    }

    private static string FrameworkSuffix()
    {
        // AppContext.TargetFrameworkName 形如 ".NETCoreApp,Version=v10.0"，取主版本号即可。
        string? target = AppContext.TargetFrameworkName;
        int marker = target?.IndexOf("Version=v", StringComparison.Ordinal) ?? -1;
        if (marker < 0)
        {
            return $"net{Environment.ProcessId}";
        }

        string version = target![(marker + "Version=v".Length)..];
        int dot = version.IndexOf('.');
        return dot > 0 ? $"net{version[..dot]}" : $"net{version}";
    }

    private static string Sanitize(string value)
    {
        char[] buffer = value.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
            {
                buffer[i] = '_';
            }
        }

        return new string(buffer);
    }

    private static string BuildConnectionString(string databaseName)
    {
        string? template = Environment.GetEnvironmentVariable(ConnectionStringVariable);
        if (string.IsNullOrWhiteSpace(template))
        {
            template = DefaultConnectionStringTemplate;
        }

        SqlConnectionStringBuilder builder = new(template.Replace("{Database}", databaseName, StringComparison.OrdinalIgnoreCase))
        {
            InitialCatalog = databaseName,
        };
        return builder.ConnectionString;
    }

    private static void DropIfExists(string databaseName, string connectionString)
    {
        SqlConnection.ClearAllPools();
        SqlConnectionStringBuilder masterBuilder = new(connectionString) { InitialCatalog = "master" };
        using SqlConnection connection = new(masterBuilder.ConnectionString);
        connection.Open();
        using SqlCommand command = connection.CreateCommand();
        // 先踢掉占用（上一次跑崩可能留下未释放的连接），否则 DROP 会失败。
        command.CommandText =
            $"""
             IF DB_ID(N'{databaseName}') IS NOT NULL
             BEGIN
                 ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                 DROP DATABASE [{databaseName}];
             END
             """;
        command.ExecuteNonQuery();
    }

    private void TryDrop()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(KeepDatabaseVariable)))
        {
            Console.Error.WriteLine($"[集成测试] 按 {KeepDatabaseVariable} 的要求保留测试数据库 {Name}。");
            return;
        }

        try
        {
            DropIfExists(Name, ConnectionString);
        }
        catch (Exception ex)
        {
            // 进程退出阶段，尽力而为；删不掉也不应改变测试结果。
            Console.Error.WriteLine($"[集成测试] 删除测试数据库 {Name} 失败：{ex.Message}");
        }
    }
}
