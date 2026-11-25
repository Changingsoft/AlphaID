using AlphaId.EntityFramework;
using Microsoft.EntityFrameworkCore;
//EF Core 10 起 SqlServerDbContextOptionsBuilder 位于 Infrastructure 命名空间，
//不再是 Microsoft.EntityFrameworkCore 根命名空间下的类型。
using Microsoft.EntityFrameworkCore.Infrastructure;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

/// <summary>
/// 把「用 SQL Server 连接 AlphaID 数据库」这件事收敛到一处。
/// </summary>
/// <remarks>
/// <para>
/// 八处 <c>UseSqlServer</c>（六个自有上下文 + IdentityServer 的两个）必须给出**完全一致**的配置：
/// 少一处 <c>MigrationsAssembly</c> 或 <c>UseNetTopologySuite</c>，那个上下文就会静默地
/// 迁移出与别人不同的 schema——这类故障在集成测试里表现为「某个接口 404」，
/// 在生产上则更糟，且不报错。
/// </para>
/// <para>
/// 因此<b>请不要在别处直接调用 <c>UseSqlServer</c></b>，一律走本方法的两个重载。
/// </para>
/// </remarks>
public static class AlphaIdSqlServerOptionsExtensions
{
    /// <summary>
    /// 以 AlphaID 的标准配置连接 SQL Server。
    /// </summary>
    /// <param name="optionsBuilder">上下文选项构造器。</param>
    /// <param name="connectionString">连接字符串。</param>
    /// <returns>同一个构造器，便于链式调用。</returns>
    public static DbContextOptionsBuilder UseAlphaIdSqlServer(this DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return optionsBuilder.UseSqlServer(
            connectionString,
            sql => sql.MigrationsAssembly(MigrationAssembly.Name).UseNetTopologySuite());
    }

    /// <summary>
    /// 在<b>已经</b>调用过 <c>UseSqlServer</c> 的构造器上补齐 AlphaID 需要的额外配置。
    /// </summary>
    /// <param name="sqlServerOptionsBuilder">SQL Server 专用的选项构造器。</param>
    /// <returns>同一个构造器，便于链式调用。</returns>
    /// <remarks>
    /// IdentityServer 的 <c>ConfigureDbContext</c> 交来的是通用的
    /// <see cref="DbContextOptionsBuilder"/>，拿不到这个重载；
    /// 此时应改用 <see cref="UseAlphaIdSqlServer(DbContextOptionsBuilder, string)"/>。
    /// </remarks>
    public static SqlServerDbContextOptionsBuilder UseAlphaIdSqlServer(this SqlServerDbContextOptionsBuilder sqlServerOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(sqlServerOptionsBuilder);

        return sqlServerOptionsBuilder
            .MigrationsAssembly(MigrationAssembly.Name)
            .UseNetTopologySuite();
    }
}
