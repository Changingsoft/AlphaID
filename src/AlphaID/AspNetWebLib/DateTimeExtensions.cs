namespace AspNetWebLib;

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
        DateTime now = DateTime.UtcNow;
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
        //2017年10月1日施行的《民法总则》第二百零一条第一款规定：“按照年、月、日计算期间的，开始的当日不计入，自下一日开始计算
        int year = now.Year - dateOfBirth.Year;
        var anniversary = new DateOnly(now.Year, dateOfBirth.Month, dateOfBirth.Day); //取得当年的纪念日。
        if (now > anniversary)
            return year;
        return year - 1;
    }
}