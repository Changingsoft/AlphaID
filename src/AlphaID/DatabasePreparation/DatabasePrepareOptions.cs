namespace DatabasePreparation;

/// <summary>
/// 控制 <see cref="DatabasePreparationExecutor"/> 执行哪些阶段。
/// </summary>
/// <remarks>
/// <para>
/// 这份开关就是「集成测试」与「数据库工具」的全部差异：两者跑的是同一段代码，
/// 只是把这里的布尔值配成了不同的组合。因此新增一个阶段只需要在这里加一个开关，
/// 而不必在两个入口各写一遍（过去正是这样，于是两边渐渐长成了两套实现）。
/// </para>
/// <para>
/// 四个开关刻意都是<b>与业务无关</b>的：只要改它们就能改变行为，
/// 不需要改任何代码。「具体灌什么内置数据 / 样例数据」不在这里，
/// 而是由调用方装配初始化单元时以委托传入。
/// </para>
/// <para>
/// 默认值的取向是「安全」：建库类操作默认关闭（不可逆），内置数据默认开启（部署必需），
/// 样例数据默认关闭（生产环境不应存在）。
/// </para>
/// </remarks>
public sealed record DatabasePrepareOptions
{
    /// <summary>
    /// 生产环境的默认值：建库与迁移打开，补齐内置数据，<b>不</b>灌样例数据。
    /// </summary>
    /// <remarks>
    /// 即「把一个已有库安全地升到最新版本」所需的最小动作，通常直接对应生产配置文件的默认值。
    /// </remarks>
    public static DatabasePrepareOptions Default { get; } = new();

    /// <summary>
    /// 开发与集成测试用的默认值：先删后建，并且把样例数据也灌上。
    /// </summary>
    /// <remarks>
    /// 通常直接对应开发环境配置文件的取值。样例数据是<b>一次性</b>写入（主键冲突即失败），
    /// 因此它只有在「库是干净的」这个前提下才成立——这里的「干净」由 <see cref="DropDatabase"/> 保证。
    /// </remarks>
    public static DatabasePrepareOptions ForDevelopment { get; } = new()
    {
        DropDatabase = true,
        AddTestingData = true,
    };

    /// <summary>
    /// 是否先删除数据库。不可逆，默认 <see langword="false"/>。
    /// </summary>
    /// <remarks>
    /// 多个初始化单元指向<b>同一个</b>库时，重复删除是安全的（<c>EnsureDeletedAsync</c> 遇库不存在直接返回），
    /// 因此各单元各自删除即可，不需要额外的协调逻辑。
    /// </remarks>
    public bool DropDatabase { get; init; }

    /// <summary>
    /// 是否应用迁移，把库结构推进到当前版本。默认 <see langword="true"/>。
    /// </summary>
    public bool ApplyMigrations { get; init; } = true;

    /// <summary>
    /// 是否补齐内置数据（<b>幂等</b>，可在已有库上反复运行）。默认 <see langword="true"/>。
    /// </summary>
    public bool AddInitData { get; init; } = true;

    /// <summary>
    /// 是否把已存在、但与代码定义不一致的内置数据覆盖回代码定义。默认 <see langword="false"/>。
    /// </summary>
    /// <remarks>
    /// 默认只补缺失的、保留已存在的——运维往往能在管理界面调整这类内置数据，
    /// 工具不该把它们冲掉。只有在内置数据本身需要升级时才置为 <see langword="true"/>。
    /// </remarks>
    public bool OverwriteInitData { get; init; }

    /// <summary>
    /// 是否灌入样例数据（<b>非幂等</b>，要求库是干净的）。默认 <see langword="false"/>。
    /// </summary>
    /// <remarks>
    /// 生产环境不应存在样例数据，因此默认关闭。开启前请确认 <see cref="DropDatabase"/> 也开着。
    /// </remarks>
    public bool AddTestingData { get; init; }
}
