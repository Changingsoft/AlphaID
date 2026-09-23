namespace DatabaseTool;

internal class DatabaseExecutorOptions
{
    /// <summary>
    /// </summary>
    public bool DropDatabase { get; set; } = false;

    public bool ApplyMigrations { get; set; } = true;

    public bool ExecutePostMigrations { get; set; } = true;

    /// <summary>
    /// 指示是否补齐内置数据（初始化数据）。
    /// </summary>
    /// <remarks>
    /// 默认为true：内置数据对系统运行至关重要，缺少它客户端无法登录。
    /// 写入采用Ensure语义（存在即跳过），因此对已有数据库重复运行是安全的。
    /// </remarks>
    public bool AddInitData { get; set; } = true;

    /// <summary>
    /// 指示是否把已存在、但与代码定义不一致的内置数据覆盖回代码定义。
    /// </summary>
    /// <remarks>
    /// 默认为false：只补缺失的内置数据，已存在的一律保持原样，因为内置数据允许运维
    /// 通过数据库直接编辑。只有在内置数据本身需要升级时才置为true，
    /// 此操作会丢弃数据库中的手工调整，并重置客户端密钥。
    /// </remarks>
    public bool OverwriteInitData { get; set; } = false;

    public bool AddTestingData { get; set; } = false;
}