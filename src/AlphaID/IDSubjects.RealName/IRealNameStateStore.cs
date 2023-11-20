namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
public interface IRealNameStateStore
{
    /// <summary>
    /// 
    /// </summary>
    IQueryable<RealNameState> RealNameStates { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<RealNameState?> FindByIdAsync(string id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="realNameState"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(RealNameState realNameState);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="realNameState"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(RealNameState realNameState);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="realNameState"></param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(RealNameState realNameState);
}