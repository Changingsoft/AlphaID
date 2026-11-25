namespace AlphaId.EntityFramework;

/// <summary>
/// 本工程（也就是各 <c>DbContext</c> 与其迁移共同所在的程序集）的名称。
/// </summary>
/// <remarks>
/// <para>
/// EF Core 默认在 <c>DbContext</c> 所在的程序集里找迁移。本工程里的六个上下文因此开箱即用，
/// 但 <b>Duende 的两个上下文</b>（<c>ConfigurationDbContext</c>、<c>PersistedGrantDbContext</c>）
/// 类型住在 Duende 的程序集里，默认会去找一个根本不存在迁移的程序集——
/// 结果是 <c>Migrate()</c> 静默建出一个没有 IdentityServer 表的空库。
/// 因此凡注册这两个上下文，都必须显式指定本工程。
/// </para>
/// <para>
/// 所有 <c>UseSqlServer</c> 调用都应经 <see cref="AlphaIdSqlServerOptionsExtensions"/> 走，
/// 由它统一指定，不要在各处写字符串字面量 <c>"AlphaId.EntityFramework"</c>：
/// 程序集一旦改名，字面量不会报错，只会静默找不到迁移。
/// </para>
/// </remarks>
public static class MigrationAssembly
{
    /// <summary>迁移所在程序集的名称。</summary>
    public static string Name { get; } = typeof(MigrationAssembly).Assembly.GetName().Name!;
}
