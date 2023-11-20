namespace IntegrationTestUtilities;

/// <summary>
/// 提供测试用时间。
/// </summary>
public class StubTimeProvider : TimeProvider
{
    private readonly DateTimeOffset time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    public StubTimeProvider(DateTimeOffset time)
    {
        this.time = time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override DateTimeOffset GetUtcNow()
    {
        return this.time.ToUniversalTime();
    }
}
