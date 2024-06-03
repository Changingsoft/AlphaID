namespace IdSubjects.RealName.Requesting;

/// <summary>
///     一个空的实名认证审核器提供器。
/// </summary>
internal class EmptyRealNameAuditorProvider : IRealNameRequestAuditorProvider
{
    /// <summary>
    ///     获取审核器。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IRealNameRequestAuditor> GetAuditors()
    {
        return [];
    }
}