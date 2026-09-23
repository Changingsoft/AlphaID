namespace AlphaId.Migrations;

/// <summary>
/// 迁移所在程序集的坐标。
/// </summary>
/// <remarks>
/// <para>
/// EF Core 需要一个稳定的「迁移在哪个程序集」标识：应用与 <c>DatabaseTool</c> 在
/// <c>UseSqlServer(..., sql =&gt; sql.MigrationsAssembly(...))</c> 里传的就是它，测试建库时同理。
/// </para>
/// <para>
/// 不要在这几处写字符串字面量 <c>"AlphaId.Migrations"</c>：程序集一旦改名，字面量不会报错，
/// 只会静默地找不到迁移（<c>Migrate()</c> 建出空库），排查成本很高。用这个属性即可在编译期把两处绑在一起。
/// </para>
/// </remarks>
public static class MigrationAssembly
{
    /// <summary>迁移所在的程序集名称。</summary>
    public static string Name { get; } = typeof(MigrationAssembly).Assembly.GetName().Name!;
}
