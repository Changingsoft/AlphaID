namespace AlphaIDPlatform;

/// <summary>
/// 针对日期时间的扩展方法。
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 以年龄的方式显示。
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    public static int? AsAge(this DateTime? dateOfBirth)
    {
        if (dateOfBirth == null)
            return null;
        return AsAge(dateOfBirth.Value);
    }

    /// <summary>
    /// 以年龄的方式显示。
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    public static int AsAge(this DateTime dateOfBirth)
    {
        var now = DateTime.UtcNow;
        return AsAge(dateOfBirth, now);
    }

    /// <summary>
    /// 以年龄的方式显示。
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    public static int? AsAge(this DateOnly? dateOfBirth)
    {
        if (dateOfBirth == null)
            return null;
        return AsAge(dateOfBirth.Value);
    }

    /// <summary>
    /// 以年龄的方式显示。
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    public static int AsAge(this DateOnly dateOfBirth)
    {
        return AsAge(dateOfBirth, DateOnly.FromDateTime(DateTime.Now));
    }

    /// <summary>
    /// For test only.
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    internal static int AsAge(this DateTime dateOfBirth, DateTime now)
    {
        return AsAge(DateOnly.FromDateTime(dateOfBirth), DateOnly.FromDateTime(now));
    }

    internal static int AsAge(this DateOnly dateOfBirth, DateOnly now)
    {
        var year = now.Year - dateOfBirth.Year;
        if (now > dateOfBirth)
            return year;
        else
            return year - 1;
    }
}
