namespace IdSubjects.RealName.Requesting;

/// <summary>
/// 实名认证请求存取器。
/// </summary>
public interface IRealNameRequestStore
{
    /// <summary>
    /// 获取一个可查询的实名认证请求集合。
    /// </summary>
    IQueryable<RealNameRequest> Requests { get; }

    /// <summary>
    /// 创建一个实名认证请求。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(RealNameRequest request);

    /// <summary>
    /// 更新实名认证请求信息。
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(RealNameRequest request);

    /// <summary>
    /// 通过 Id 查找实名认证请求。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<RealNameRequest?> FindByIdAsync(int id);
}