namespace IDSubjects.RealName;

/// <summary>
/// 提供查找实名认证请求的能力。
/// </summary>
public interface IChineseIdCardValidationStore
{
    /// <summary>
    /// 获取实名认证信息的可查询集合。
    /// </summary>
    IQueryable<ChineseIdCardValidation> RealNameValidations { get; }

    /// <summary>
    /// 根据自然人查找待认证的请求。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<ChineseIdCardValidation?> GetPendingRequestAsync(NaturalPerson person);

    /// <summary>
    /// 获取当前已通过认证的实名信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<ChineseIdCardValidation?> GetCurrentAsync(NaturalPerson person);

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task CreateAsync(ChineseIdCardValidation request);

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task UpdateAsync(ChineseIdCardValidation request);

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task DeleteAsync(ChineseIdCardValidation request);

    /// <summary>
    /// Find validation by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    ValueTask<ChineseIdCardValidation?> FindByIdAsync(int id);
}