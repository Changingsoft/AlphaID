using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSubjects.RealName;
/// <summary>
/// 表示一个实名验证请求
/// </summary>
public class RealNameValidationRequest
{
    public int Id { get; protected set; }

    public DateTimeOffset WhenCommited { get; protected set; }


}
