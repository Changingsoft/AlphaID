namespace IdSubjects.RealName.Requesting;
/// <summary>
/// 表示一个实名验证请求
/// </summary>
public class RealNameValidationRequest
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset WhenCommitted { get; protected set; }


}
