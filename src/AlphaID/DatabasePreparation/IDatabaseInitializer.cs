using Microsoft.EntityFrameworkCore;

namespace DatabasePreparation;

/// <summary>
/// 一个 <see cref="DbContext"/> 在「准备数据库」这件事上的<b>可装配单元</b>。
/// </summary>
/// <remarks>
/// <para>
/// 设计意图是把建库流程按 <see cref="DbContext"/> 拆开：每个上下文各自负责「我能做哪几步」，
/// 由 <see cref="DatabasePreparationExecutor"/> 统一按阶段顺序驱动它们。
/// 这样新增一个 <see cref="DbContext"/> 时，只需要写一个实现本接口的类并注册进去，
/// <b>不需要改动执行器，也不需要为它重写一遍建库流程</b>——这正是本接口存在的意义。
/// </para>
/// <para>
/// 四个方法的<b>默认实现都是空操作</b>，因此只关心其中一两步的实现不必写空方法：
/// 一个只提供迁移的上下文，实现 <see cref="MigrateAsync"/> 即可。
/// </para>
/// <para>
/// 这套抽象不依赖任何业务工程：内置数据与样例数据等业务概念都通过委托从外部注入，
/// 因此可以直接复制到其他项目复用。
/// </para>
/// </remarks>
public interface IDatabaseInitializer
{
    /// <summary>
    /// 本单元负责的 <see cref="DbContext"/>。
    /// </summary>
    DbContext DbContext { get; }

    /// <summary>
    /// 本单元的显示名，用于日志与异常信息。
    /// </summary>
    /// <remarks>
    /// 默认取 <see cref="DbContext"/> 的类型名。对多上下文项目而言，
    /// 日志里能区分出「这一步是哪个上下文做的」很重要。
    /// </remarks>
    string Name => DbContext.GetType().Name;

    /// <summary>
    /// 本单元是否<b>真的</b>负责灌入样例数据。
    /// </summary>
    /// <remarks>
    /// 默认 <see langword="false"/>：多数上下文确实不管这件事。
    /// 执行器据此在动手之前判断「开了 <see cref="DatabasePrepareOptions.AddTestingData"/>
    /// 却没有任何单元管它」，把配置错误挡在数据灌了一半之前。
    /// 注意：仅仅<b>继承了</b>默认的空实现不算，必须显式声明职责。
    /// </remarks>
    bool HandlesTestingData => false;

    /// <summary>
    /// 本单元是否<b>真的</b>负责补齐内置数据。
    /// </summary>
    /// <remarks>
    /// 默认 <see langword="false"/>。主要用于自检与日志；与
    /// <see cref="HandlesTestingData"/> 不同，它<b>不会</b>触发执行器的拦截——
    /// 「没有任何上下文有内置数据」是完全合法的（例如新项目尚未定义任何内置数据）。
    /// </remarks>
    bool HandlesInitData => false;

    /// <summary>
    /// 删除该上下文所指向的数据库。
    /// </summary>
    /// <param name="cancellationToken">取消标记。</param>
    /// <remarks>
    /// 多个上下文指向<b>同一个</b>库时，重复删除是安全的：<c>EnsureDeletedAsync</c> 遇到库不存在直接返回。
    /// 因此不需要额外的「只删一次」协调逻辑，各上下文独立删除即可。
    /// </remarks>
    Task DropDatabaseAsync(CancellationToken cancellationToken = default)
        => DbContext.Database.EnsureDeletedAsync(cancellationToken);

    /// <summary>
    /// 应用该上下文的迁移，把库结构推进到当前版本。
    /// </summary>
    /// <param name="cancellationToken">取消标记。</param>
    /// <remarks>
    /// 迁移与 <see cref="DbContext"/> 同处一个程序集时，EF 默认就在该程序集里找迁移，
    /// 无需再指定 <c>MigrationsAssembly</c>。各上下文的迁移写进同一个
    /// <c>__EFMigrationsHistory</c>，彼此不会重复执行。
    /// </remarks>
    Task MigrateAsync(CancellationToken cancellationToken = default)
        => DbContext.Database.MigrateAsync(cancellationToken);

    /// <summary>
    /// 补齐本上下文所属的<b>内置数据</b>（<b>幂等</b>：缺失则插入、已存在则跳过）。
    /// </summary>
    /// <param name="options">本次执行的阶段开关，内置数据的写入策略（如是否覆盖）从中读取。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <remarks>
    /// 默认不写任何内置数据——大多数上下文本来就没有。
    /// </remarks>
    Task SeedInitDataAsync(DatabasePrepareOptions options, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>
    /// 灌入本上下文所属的<b>样例数据</b>（<b>非幂等</b>：要求库是干净的）。
    /// </summary>
    /// <param name="cancellationToken">取消标记。</param>
    /// <remarks>
    /// 默认不灌任何样例数据。生产环境不应存在样例数据，执行器只在
    /// <see cref="DatabasePrepareOptions.AddTestingData"/> 为 <see langword="true"/> 时才调用本方法。
    /// </remarks>
    Task SeedTestingDataAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}