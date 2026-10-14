namespace DatabaseTool;

/// <summary>
/// 数据库工具的配置约定。
/// </summary>
internal static class DatabaseToolOptions
{
    /// <summary>
    /// 存放 <see cref="DatabasePrepareOptions"/> 的配置节名。
    /// </summary>
    /// <remarks>
    /// 旧版叫 <c>DatabaseExecutorOptions</c>。改名是为了让它与类型同名：
    /// 过去工具与测试各有一套开关（工具独有的 <c>ExecutePostMigrations</c> 实际上是个死开关），
    /// 现在两边共用同一个类型，不存在「两套开关」的可能。
    /// 旧节名仍会被读取，同一键上新节优先，既有部署脚本不必立刻改。
    /// </remarks>
    public const string SectionName = "DatabasePreparationOptions";

    /// <summary>旧配置节名，仅为兼容既有部署脚本而保留。</summary>
    public const string LegacySectionName = "DatabaseExecutorOptions";
}
