using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore
{
    /// <summary>
    /// 表示一个日期时间条件。
    /// </summary>
    public class DateTimeCondition : RadiusCondition
    {
        [MaxLength(24)]
        public virtual byte[] Matrix { get; set; } = new byte[24];

        public override bool TestCondition(RadiusContext context)
        {
            // 0: Sunday, 1: Monday, 2: Tuesday, 3: Wednesday, 4: Thursday, 5: Friday, 6: Saturday
            var weekday = (int)DateTime.Now.DayOfWeek;
            var hour = DateTime.Now.Hour;

            var hourByte = Matrix[hour];
            return ((hourByte << weekday) & 0x01) == 1;
        }
    }
}
