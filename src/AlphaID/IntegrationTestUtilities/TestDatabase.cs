using AlphaId.DatabasePreparation;
using AlphaId.EntityFramework.InitData;
using DatabasePreparation;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
/// 现在改为：每个测试进程建一个**唯一命名**的库，跑一遍 <see cref="DatabasePreparationExecutor"/>——
/// <b>与 DatabaseTool 完全相同的那一套</b>——再在进程退出时删除。于是测试既不依赖开发库，
/// 也不需要 DatabaseTool，并且每次运行都从同一个已知状态出发，这正是「可回归」的含义。
/// </para>
/// <para>
/// 库名字固定为 <c>AlphaIdTest-{测试程序集}-net{主版本}</c>（而不是每次随机），这样重复运行会**重建**同一个库，
/// 既不会积累垃圾库，也不会被上一次跑崩留下的脏数据影响。想保留现场便于排查时，设置环境变量
/// <see cref="KeepDatabaseVariable"/> 即可跳过退出时的删除。
/// </para>
/// <para>
/// 唯一与 DatabaseTool 不同的地方是：删库走 <see cref="ForceDrop"/>（强制踢掉占用连接），
/// 而不是 <c>EnsureDeleted</c>。上一次跑崩留下的未释放连接会让 LocalDB 上的 DROP 失败，
/// 那属于测试环境的固有问题，不该由生产用的建库逻辑来兜。
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
    /// 在测试宿主下入口程序集往往是 <c>testhost</c>，多个测试项目会因此撞到同一个库名。
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
        ForceDrop(name, connectionString);

        using ServiceProvider provider = BuildServices(connectionString);
        using IServiceScope scope = provider.CreateScope();
        Prepare(scope.ServiceProvider);
        VerifyInitDataRoundTrip(scope.ServiceProvider);

        TestDatabase database = new(name, connectionString);
        AppDomain.CurrentDomain.ProcessExit += (_, _) => database.TryDrop();
        return database;
    }

    /// <summary>
    /// 装配服务。建库流程本身交给 <see cref="Prepare"/>，与 DatabaseTool 共用。
    /// </summary>
    /// <remarks>
    /// 不自己 <c>new DbContextOptions&lt;T&gt;()</c> 的理由有两个：
    /// 一是 Duende 的 <c>ConfigurationDbContext</c> 会从容器里取 <c>ConfigurationStoreOptions</c>，
    /// 裸造上下文会以 "Unable to resolve service for type ... ConfigurationStoreOptions" 失败；
    /// 二是走 <c>AddAlphaIdDatabasePreparation</c> 这同一套装配，才能保证测试建的库与
    /// <c>DatabaseTool</c> 建的库 schema 完全一致——这正是两者共用一套逻辑的意义。
    /// </remarks>
    private static ServiceProvider BuildServices(string connectionString)
    {
        ServiceCollection services = new();
        services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        services.AddAlphaIdDatabasePreparation(connectionString);
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// 跑迁移 → 内置数据 → 样例数据三个阶段，用的是与 DatabaseTool 相同的执行器与同一套装配。
    /// </summary>
    /// <param name="services">已建好的服务作用域。</param>
    /// <remarks>
    /// <see cref="DatabasePrepareOptions.DropDatabase"/> 刻意<b>不</b>打开：
    /// <see cref="ForceDrop"/> 已经用 SINGLE_USER 强制删过，这里再让每个上下文各自
    /// <c>EnsureDeleted</c> 一次既多余又可能在连接池有残留时失败。
    /// </remarks>
    private static void Prepare(IServiceProvider services)
    {
        DatabasePreparationExecutor executor = services.GetRequiredService<DatabasePreparationExecutor>();
        executor.ExecuteAsync(new DatabasePrepareOptions
        {
            DropDatabase = false,
            ApplyMigrations = true,
            AddInitData = true,
            AddTestingData = true,
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 证明「刚灌完的库与代码定义完全一致」，并顺带核对八个上下文都被建了出来。
    /// </summary>
    /// <param name="services">已建好的服务作用域。</param>
    /// <remarks>
    /// 紧接着再跑一次 <c>EnsureAsync</c>，此时所有行都已存在，必须一行都不插、一条差异告警都没有。
    /// 若出现告警，说明某个字段经数据库往返后不再是代码里的值（长度截断、大小写折叠、时间精度丢失之类），
    /// 这类问题如果放到业务测试里才暴露会非常难查。
    /// </remarks>
    private static void VerifyInitDataRoundTrip(IServiceProvider services)
    {
        // 逐个核对：某个上下文若被漏注册，这里会直接指出是谁，而不会等到某个接口 404。
        foreach (Type contextType in AlphaIdDatabasePreparation.ManagedContextTypes)
        {
            if (services.GetService(contextType) is not DbContext context)
                throw new InvalidOperationException($"上下文 {contextType.Name} 未被装配，测试库会缺少它负责的表。");

            if (!context.Database.CanConnect())
                throw new InvalidOperationException($"上下文 {contextType.Name} 无法连接新建的测试库。");
        }

        ConfigurationDbContext db = services.GetRequiredService<ConfigurationDbContext>();
        InitDataResult verify = InitDataSeeder.EnsureAsync(db).GetAwaiter().GetResult();
        if (verify.Inserted != 0 || verify.Warnings.Count != 0)
        {
            throw new InvalidOperationException(
                $"内置数据经数据库往返后与代码定义不一致：新插入 {verify.Inserted} 行。" +
                Environment.NewLine + string.Join(Environment.NewLine, verify.Warnings));
        }
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

    /// <summary>
    /// 强制删除数据库：先踢掉占用，再 DROP。
    /// </summary>
    /// <param name="databaseName">库名。</param>
    /// <param name="connectionString">连接字符串；只取其中的服务器部分。</param>
    /// <remarks>
    /// 不直接用 <c>EnsureDeleted</c>：上一次跑崩留下的未释放连接会让 DROP 失败。
    /// </remarks>
    public static void ForceDrop(string databaseName, string connectionString)
    {
        SqlConnection.ClearAllPools();
        SqlConnectionStringBuilder masterBuilder = new(connectionString) { InitialCatalog = "master" };
        using SqlConnection connection = new(masterBuilder.ConnectionString);
        connection.Open();
        using SqlCommand command = connection.CreateCommand();
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
            ForceDrop(Name, ConnectionString);
        }
        catch (Exception ex)
        {
            // 进程退出阶段，尽力而为；删不掉也不应改变测试结果。
            Console.Error.WriteLine($"[集成测试] 删除测试数据库 {Name} 失败：{ex.Message}");
        }
    }
}
