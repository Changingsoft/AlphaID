namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 
/// </summary>
public interface IRealNameRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(RealNameRequest request);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(RealNameRequest request);

    /// <summary>
    /// 
    /// </summary>
    IQueryable<RealNameRequest> Requests { get; }
}