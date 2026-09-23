using AlphaId.InitData;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace IntegrationTestUtilities;

/// <summary>
/// 集成测试用的内置数据（初始化数据）入口。
/// </summary>
/// <remarks>
/// <para>
/// 内置数据（IdentityServer 的客户端、API 范围、标识资源）对系统运行至关重要：
/// 缺少客户端，授权码流程与令牌端点都会失败。此前集成测试依赖「数据库碰巧被
/// <c>DatabaseTool</c> 灌过」这一隐式前提，测试项目本身对此零引用，一旦在全新数据库上
/// 运行就会以难以理解的方式失败。
/// </para>
/// <para>
/// 这里在测试宿主启动后补齐内置数据。<b>只补齐缺失的部分，不覆盖已有数据，也不会建库或删库</b>：
/// 集成测试目前仍与开发数据库共用同一个库，写入必须保持保守。
/// 每个连接字符串在一个测试进程内只会执行一次。
/// </para>
/// </remarks>
public static class InitDataTestExtensions
{
    private static readonly ConcurrentDictionary<string, Lazy<InitDataResult>> Results = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 确保当前测试宿主所连接的数据库中，IdentityServer 的内置数据齐全。
    /// </summary>
    /// <param name="services">测试宿主的服务提供程序（通常是 <c>WebApplicationFactory.Services</c> 或 <c>IHost.Services</c>）。</param>
    /// <returns>内置数据的补齐结果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="InvalidOperationException">宿主未注册 <see cref="ConfigurationDbContext"/>。</exception>
    public static InitDataResult EnsureInitData(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        string key = ResolveConnectionString(services);
        return Results
            .GetOrAdd(key, _ => new Lazy<InitDataResult>(() => Ensure(services), LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;
    }

    private static string ResolveConnectionString(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        ConfigurationDbContext db = ResolveConfigurationDbContext(scope.ServiceProvider);
        return db.Database.GetConnectionString() ?? nameof(ConfigurationDbContext);
    }

    private static InitDataResult Ensure(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        ConfigurationDbContext db = ResolveConfigurationDbContext(scope.ServiceProvider);

        // 测试宿主没有同步上下文，这里同步等待是安全的。
        InitDataResult result = InitDataSeeder.EnsureAsync(db).GetAwaiter().GetResult();
        if (result.Warnings.Count > 0)
        {
            Console.Error.WriteLine(
                $"[内置数据] 数据库中的内置数据与代码定义存在差异（未自动修改）：{Environment.NewLine}" +
                string.Join(Environment.NewLine, result.Warnings));
        }
        return result;
    }

    private static ConfigurationDbContext ResolveConfigurationDbContext(IServiceProvider services) =>
        services.GetService<ConfigurationDbContext>()
        ?? throw new InvalidOperationException(
            $"测试宿主没有注册 {nameof(ConfigurationDbContext)}，无法补齐 IdentityServer 内置数据。" +
            "请在应用的启动代码中调用 AddConfigurationStore 或 AddDbContext<ConfigurationDbContext>。");
}
