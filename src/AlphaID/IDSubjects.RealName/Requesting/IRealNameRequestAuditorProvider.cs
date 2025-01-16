namespace IdSubjects.RealName.Requesting;

/// <summary>
///     表示一个实名请求审核器的提供器接口。
/// </summary>
public interface IRealNameRequestAuditorProvider
{
    /// <summary>
    ///     获取审核器。
    /// </summary>
    /// <returns></returns>
    IEnumerable<IRealNameRequestAuditor> GetAuditors();
}