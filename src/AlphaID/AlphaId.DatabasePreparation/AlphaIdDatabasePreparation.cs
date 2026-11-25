using AlphaId.EntityFramework;
using AlphaId.EntityFramework.Admin;
using AlphaId.EntityFramework.DirectoryAccountManagement;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.EntityFramework.InitData;
using AlphaId.EntityFramework.RealName;
using AlphaId.EntityFramework.SecurityAuditing;
using AlphaId.TestingData;
using DatabasePreparation;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlphaId.DatabasePreparation;

/// <summary>
/// AlphaID 的「数据库准备」装配：把上下文、内置数据与样例数据接到 <see cref="DatabasePreparationExecutor"/> 上。
/// </summary>
/// <remarks>
/// <para>
/// <b>本工程是通用抽象 <c>DatabasePreparation</c> 与 AlphaID 之间的那一层适配</b>，
/// 也是唯一一个知道「AlphaID 有哪些上下文、内置数据和样例数据」的地方。
/// 这样切分是为了让 <c>DatabasePreparation</c> 保持零业务依赖——它可以被整目录复制到
/// 其他解决方案、作为独立 NuGet 包发布，而不必拖走 AlphaID 的任何概念。
/// 换句话说：<b>策略（有哪些表、灌什么数据）住在这里，机制（阶段顺序、删库与迁移）住在抽象层</b>。
/// </para>
/// <para>
/// <b>DatabaseTool 与集成测试的唯一差别，就是这里传进去的连接字符串与
/// <see cref="DatabasePrepareOptions"/> 的取值</b>，其余代码完全共用。
/// 过去两者各写一套（工具是 <c>DatabaseExecutor</c> + 8 个 <c>DatabaseMigrator</c>，
/// 测试是 <c>TestDatabase</c> 里手写的三个私有方法），于是两边迟早会悄悄长偏：
/// 工具多灌了什么、测试少跑了一步，都不会有任何提示。
/// </para>
/// <para>
/// 样例数据住在 <c>AlphaId.TestingData</c>（仅开发调试与集成测试使用），由本工程以委托接入；
/// 产品程序集（WebApp / WebAPI）不引用本工程，因此样例数据不会进入生产。
/// </para>
/// <para>
/// 新增一个 <see cref="DbContext"/> 只需在 <see cref="AddAlphaIdDatabasePreparation"/> 里
/// 加一个初始化单元，工具与集成测试<b>同时</b>获得它，不必改动任何一方。
/// </para>
/// </remarks>
public static class AlphaIdDatabasePreparation
{
    /// <summary>
    /// 装配 AlphaID 全部数据库上下文，以及驱动它们所需的初始化单元。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="connectionString">
    /// 连接字符串。八个上下文<b>共用</b>它，因此也共用同一个库；
    /// 重复删库/重复迁移都是安全的。
    /// </param>
    /// <returns>同一个服务集合，便于链式调用。</returns>
    public static IServiceCollection AddAlphaIdDatabasePreparation(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddAlphaIdPlatform()
            .AddEntityFramework(options => options.UseAlphaIdSqlServer(connectionString));

        //IdentityServer 的配置库与授权库。它们是 Duende 的上下文，迁移在 AlphaId.EntityFramework，
        //因此必须显式指定迁移程序集（已收敛进 UseAlphaIdSqlServer）。
        services.AddIdentityServer()
            .AddConfigurationStore(options =>
                options.ConfigureDbContext = builder => builder.UseAlphaIdSqlServer(connectionString))
            .AddOperationalStore(options =>
                options.ConfigureDbContext = builder => builder.UseAlphaIdSqlServer(connectionString));

        //内置数据：IdentityServer 的客户端、API 范围与标识资源。
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<ConfigurationDbContext>(
            sp.GetRequiredService<ConfigurationDbContext>(),
            seedInitData: (db, options, ct) => EnsureInitDataAsync(db, options, sp.GetService<ILoggerFactory>(), ct),
            name: "IdentityServer配置库"));

        //样例数据：自然人、组织与角色分配。
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<AlphaIdIdentityDbContext>(
            sp.GetRequiredService<AlphaIdIdentityDbContext>(),
            seedTestingData: (db, ct) => SampleDataSeeder.SeedAsync(db, cancellationToken: ct),
            name: "身份库"));
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<AlphaIdDbContext>(
            sp.GetRequiredService<AlphaIdDbContext>(),
            seedTestingData: (db, ct) => SampleDataSeeder.SeedAsync(db, cancellationToken: ct),
            name: "平台库"));
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<OperationalDbContext>(
            sp.GetRequiredService<OperationalDbContext>(),
            seedTestingData: (db, ct) => SampleDataSeeder.SeedAsync(db, cancellationToken: ct),
            name: "管理库"));

        //其余上下文只参与删库与迁移，不持有任何数据。
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<PersistedGrantDbContext>(
            sp.GetRequiredService<PersistedGrantDbContext>(), name: "IdentityServer授权库"));
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<DirectoryLogonDbContext>(
            sp.GetRequiredService<DirectoryLogonDbContext>(), name: "目录登录库"));
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<RealNameDbContext>(
            sp.GetRequiredService<RealNameDbContext>(), name: "实名库"));
        services.AddScoped<IDatabaseInitializer>(sp => new DbContextInitializer<LoggingDbContext>(
            sp.GetRequiredService<LoggingDbContext>(), name: "审计日志库"));

        services.AddScoped<DatabasePreparationExecutor>();
        return services;
    }

    /// <summary>
    /// AlphaID 全部受管的 <see cref="DbContext"/>，按迁移执行顺序排列。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="AddAlphaIdDatabasePreparation"/> 注册的初始化单元一一对应。
    /// 集成测试用它来核对「该建的库都建了」，避免某个上下文被漏注册却无人察觉。
    /// </remarks>
    public static IReadOnlyList<Type> ManagedContextTypes { get; } =
    [
        typeof(ConfigurationDbContext),
        typeof(PersistedGrantDbContext),
        typeof(AlphaIdIdentityDbContext),
        typeof(DirectoryLogonDbContext),
        typeof(RealNameDbContext),
        typeof(OperationalDbContext),
        typeof(LoggingDbContext),
        typeof(AlphaIdDbContext),
    ];

    private static async Task EnsureInitDataAsync(
        ConfigurationDbContext db,
        DatabasePrepareOptions options,
        ILoggerFactory? loggerFactory,
        CancellationToken cancellationToken)
    {
        ILogger? logger = loggerFactory?.CreateLogger(typeof(AlphaIdDatabasePreparation));
        InitDataResult result = await InitDataSeeder.EnsureAsync(
            db,
            new InitDataEnsureOptions { OverwriteExisting = options.OverwriteInitData },
            cancellationToken).ConfigureAwait(false);

        if (result.Inserted > 0)
            logger?.LogInformation("已补齐 {Count} 项 IdentityServer 内置数据。", result.Inserted);
        else
            logger?.LogInformation("IdentityServer 内置数据已存在，无需写入。");

        foreach (string warning in result.Warnings)
            logger?.LogWarning("{Warning}", warning);
    }
}
