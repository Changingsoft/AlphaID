using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabasePreparation;

/// <summary>
/// 统一驱动所有 <see cref="IDatabaseInitializer"/> 完成「删库 → 迁移 → 内置数据 → 样例数据」四个阶段。
/// </summary>
/// <remarks>
/// <para>
/// 执行器<b>不认识</b>任何具体的 <see cref="DbContext"/>：它只认「一组初始化单元」，
/// 并保证「所有单元都做完第 1 阶段，再都做第 2 阶段」——
/// 也就是<b>阶段之间</b>存在全局屏障，<b>阶段内部</b>按注册顺序进行。
/// </para>
/// <para>
/// 这个顺序是硬性的，因为每一步都依赖前一步的结果：
/// </para>
/// <list type="number">
/// <item><description>
/// <b>删库</b>必须最先，否则残留的表结构会与迁移冲突。
/// </description></item>
/// <item><description>
/// <b>迁移</b>必须早于任何数据写入，否则表还不存在。
/// </description></item>
/// <item><description>
/// <b>内置数据</b>必须早于<b>样例数据</b>：样例数据往往引用内置数据里的记录
/// （例如短信任务引用的客户端 Id），顺序颠倒会让样例数据指向一个不存在的实体。
/// </description></item>
/// </list>
/// <para>
/// 因此新增一个 <see cref="DbContext"/> 只需注册一个 <see cref="IDatabaseInitializer"/>，
/// 执行器不需要任何改动；反过来，调用方也无法绕过阶段顺序——
/// 顺序只在这里写了一份。
/// </para>
/// </remarks>
public sealed class DatabasePreparationExecutor(
    IEnumerable<IDatabaseInitializer> initializers,
    ILogger<DatabasePreparationExecutor>? logger = null)
{
    private readonly IDatabaseInitializer[] _initializers =
        initializers?.ToArray() ?? throw new ArgumentNullException(nameof(initializers));

    /// <summary>
    /// 取得当前装配的初始化单元，按执行顺序排列。
    /// </summary>
    /// <remarks>
    /// 暴露出来是为了让调用方能自检（例如集成测试断言「两个上下文都已被纳入」），
    /// 而不必把集合字段暴露成可写状态。
    /// </remarks>
    public IReadOnlyList<IDatabaseInitializer> Initializers => _initializers;

    /// <summary>
    /// 按 <paramref name="options"/> 指定的阶段准备数据库。
    /// </summary>
    /// <param name="options">阶段开关；为 <see langword="null"/> 时使用 <see cref="DatabasePrepareOptions.Default"/>。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>表示准备已完成的任务。</returns>
    /// <exception cref="InvalidOperationException">要求灌样例数据，却没有任何初始化单元负责该步骤。</exception>
    public async Task ExecuteAsync(
        DatabasePrepareOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= DatabasePrepareOptions.Default;

        //开关与能力不匹配时必须立刻失败：等到「数据已经灌了一半」才发现，
        //留下的半截库比直接抛异常难收拾得多。
        if (options.AddTestingData && !_initializers.Any(initial => initial.HandlesTestingData))
        {
            throw new InvalidOperationException(
                "开启了 AddTestingData，但没有任何初始化单元负责灌入样例数据。" +
                "样例数据通常住在独立项目里，必须由调用方显式装配进来。");
        }

        //Step1: 删库。各上下文指向同一个库时重复删除是安全的（EnsureDeletedAsync 遇库不存在直接返回）。
        if (options.DropDatabase)
        {
            logger?.LogInformation("第1阶段：正在删除数据库。");
            foreach (IDatabaseInitializer initializer in _initializers)
            {
                await initializer.DropDatabaseAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        //Step2: 迁移。
        if (options.ApplyMigrations)
        {
            logger?.LogInformation("第2阶段：正在应用数据库迁移。");
            foreach (IDatabaseInitializer initializer in _initializers)
            {
                await initializer.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        //Step3: 补齐内置数据（幂等）。
        if (options.AddInitData)
        {
            logger?.LogInformation("第3阶段：正在补齐内置数据。");
            foreach (IDatabaseInitializer initializer in _initializers)
            {
                await initializer.SeedInitDataAsync(options, cancellationToken).ConfigureAwait(false);
            }
        }

        //Step4: 灌入样例数据（非幂等，要求库是干净的）。
        if (options.AddTestingData)
        {
            logger?.LogInformation("第4阶段：正在写入样例数据。");
            foreach (IDatabaseInitializer initializer in _initializers)
            {
                await initializer.SeedTestingDataAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}