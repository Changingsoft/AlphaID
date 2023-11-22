namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public interface IRealNameRequestAuditorProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerable<IRealNameRequestAuditor> GetAuditors();
}