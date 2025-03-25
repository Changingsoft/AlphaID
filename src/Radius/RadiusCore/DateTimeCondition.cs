using System.ComponentModel.DataAnnotations;

namespace RadiusCore;

/// <summary>
/// 表示一个日期时间条件。
/// </summary>
public class DateTimeCondition : RadiusCondition
{
    /// <summary>
    /// 获取或设置条件矩阵。
    /// </summary>
    /// <remarks>
    /// 条件矩阵是一个24字节的数组，每一个字节代表一个小时，每个字节从最低位起算，分别代表周日、周一、周二、周三、周四、周五、周六，第8位未使用。
    /// </remarks>
    [MaxLength(24)]
    public virtual byte[] Matrix { get; set; } = new byte[24];

    internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// 测试条件是否为真。
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool TestCondition(RadiusContext context)
    {
        var now = TimeProvider.GetLocalNow();
        // 0: Sunday, 1: Monday, 2: Tuesday, 3: Wednesday, 4: Thursday, 5: Friday, 6: Saturday
        var weekday = (int)now.DayOfWeek;
        var hour = now.Hour;

        var hourByte = Matrix[hour];
        return ((hourByte >> weekday) & 0x01) == 1;
    }
}
