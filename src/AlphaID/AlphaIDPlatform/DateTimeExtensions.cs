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
    /// For test only.
    /// </summary>
    /// <param name="dateOfBirth"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    internal static int AsAge(this DateTime dateOfBirth, DateTime now)
    {
        var year = (now.Year - dateOfBirth.Year);
        var birthDay = new DateTime(now.Year, dateOfBirth.Month, dateOfBirth.Day);
        if (now.Date > birthDay)
            return year;
        else
            return year - 1;
    }
}
