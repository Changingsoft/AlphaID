namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public interface IRealNameRequestAuditor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> ValidateAsync(RealNameRequest request);
}