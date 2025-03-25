namespace RadiusCore.Tests;

/// <summary>
/// 提供冻结的时间以便于测试。
/// </summary>
/// <remarks>
/// 通过指定的时间创建一个冻结的TimeProvider.
/// </remarks>
/// <param name="time"></param>
public class FrozenTimeProvider(DateTimeOffset time) : TimeProvider
{
    /// <summary>
    /// 从当前UTC时间创建一个冻结时间。该时间表明创建TimeProvider时的时间，并且在接下来的声明周期里不再发生变化。
    /// </summary>
    public FrozenTimeProvider()
        : this(DateTimeOffset.UtcNow)
    {
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override DateTimeOffset GetUtcNow()
    {
        return time.ToUniversalTime();
    }
}