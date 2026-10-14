using Microsoft.EntityFrameworkCore;

namespace DatabasePreparation;

/// <summary>
/// <see cref="IDatabaseInitializer"/> 的通用实现：删库与迁移走 EF 自带行为，
/// 内置数据与样例数据由调用方以委托注入。
/// </summary>
/// <typeparam name="TContext">所负责的数据库上下文类型。</typeparam>
/// <remarks>
/// <para>
/// 这是让整套机制<b>能在其他项目复用</b>的关键：绝大多数上下文只需要「能删库、能迁移」，
/// 用本类即可，无需自己写一个实现类：
/// </para>
/// <code>
/// services.AddScoped&lt;IDatabaseInitializer&gt;(sp =&gt;
///     new DbContextInitializer&lt;MyDbContext&gt;(sp.GetRequiredService&lt;MyDbContext&gt;()));
/// </code>
/// <para>
/// 只有当某个上下文还有内置数据要补齐时才多传一个委托。
/// 本类<b>不引用</b>任何推送中心特有的类型，因此在别的解决方案里可以直接使用。
/// </para>
/// </remarks>
public sealed class DbContextInitializer<TContext>(
    TContext context,
    Func<TContext, CancellationToken, Task>? seedInitData = null,
    Func<TContext, CancellationToken, Task>? seedTestingData = null,
    string? name = null)
    : IDatabaseInitializer
    where TContext : DbContext
{
    /// <inheritdoc />
    public DbContext DbContext { get; } = context ?? throw new ArgumentNullException(nameof(context));

    /// <inheritdoc />
    public string Name { get; } = name ?? typeof(TContext).Name;

    /// <summary>
    /// 强类型的上下文。写数据委托需要它——<see cref="IDatabaseInitializer.DbContext"/>
    /// 只暴露基类，而多数 Seeder 要的是具体上下文类型。
    /// </summary>
    private TContext TypedContext { get; } = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// 本单元是否负责补齐内置数据。
    /// </summary>
    /// <remarks>
    /// 取决于构造时是否真的传入了 <c>seedInitData</c>。
    /// </remarks>
    public bool HandlesInitData => seedInitData is not null;

    /// <inheritdoc />
    /// <remarks>
    /// 取决于构造时是否真的传入了 <c>seedTestingData</c>。刻意<b>不</b>让接口的默认值蒙混过关——
    /// 否则执行器的前置校验会误判成「有人管」，于是在完全没人写数据的情况下静默通过。
    /// </remarks>
    public bool HandlesTestingData => seedTestingData is not null;

    /// <inheritdoc />
    /// <remarks>
    /// 未提供 <c>seedInitData</c> 时<b>什么都不做</b>，而不是抛异常。
    /// 这是「按上下文装配」的关键语义：大多数上下文本来就没有内置数据
    /// （例如短信库只有短信任务与验证码会话），全局开关 <c>AddInitData</c> 打开时
    /// 不应该要求每个上下文都能写内置数据。
    /// </remarks>
    public Task SeedInitDataAsync(CancellationToken cancellationToken = default)
        => seedInitData is null ? Task.CompletedTask : seedInitData(TypedContext, cancellationToken);

    /// <inheritdoc />
    /// <remarks>
    /// 未提供 <c>seedTestingData</c> 时<b>什么都不做</b>，理由同上：
    /// 并不是每个上下文都有样例数据。真正需要防的是「全局要求灌样例数据却没人管」，
    /// 那由执行器依据 <see cref="HandlesTestingData"/> 在动手之前统一拦截。
    /// </remarks>
    public Task SeedTestingDataAsync(CancellationToken cancellationToken = default)
        => seedTestingData is null ? Task.CompletedTask : seedTestingData(TypedContext, cancellationToken);
}

/// <summary>
/// <see cref="DbContextInitializer{TContext}"/> 的非泛型别名，便于在只关心接口时使用。
/// </summary>
/// <remarks>
/// 便于 DI 注册时写出「注册一批 initializer」而无需关心各自的具体上下文类型。
/// </remarks>
public static class DbContextInitializer
{
    /// <summary>
    /// 创建一个只负责删库与迁移的初始化单元。
    /// </summary>
    /// <typeparam name="TContext">数据库上下文类型。</typeparam>
    /// <param name="context">上下文实例。</param>
    /// <param name="name">显示名；为 <see langword="null"/> 时取类型名。</param>
    /// <returns>初始化单元。</returns>
    public static IDatabaseInitializer For<TContext>(TContext context, string? name = null)
        where TContext : DbContext
        => new DbContextInitializer<TContext>(context, name: name);
}