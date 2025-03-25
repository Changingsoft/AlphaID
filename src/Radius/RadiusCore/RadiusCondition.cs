namespace RadiusCore;

/// <summary>
/// 条件。
/// </summary>
public abstract class RadiusCondition
{
    /// <summary>
    /// 测试条件是否为真。
    /// </summary>
    /// <returns></returns>
    public abstract bool TestCondition(RadiusContext context);
}