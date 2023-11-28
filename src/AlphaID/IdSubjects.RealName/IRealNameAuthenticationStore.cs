namespace IdSubjects.RealName;

/// <summary>
/// 实名认证存取器接口。
/// </summary>
public interface IRealNameAuthenticationStore
{
    /// <summary>
    /// 获取可查询的实名认证信息集合。
    /// </summary>
    IQueryable<RealNameAuthentication> Authentications { get; }

    /// <summary>
    /// 创建实名认证信息。
    /// </summary>
    /// <param name="authentication">实名认证信息。</param>
    /// <returns></returns>
    Task<IdOperationResult> CreateAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 更新实名认证信息。
    /// </summary>
    /// <param name="authentication">要更新的实名认证信息。</param>
    /// <returns></returns>
    Task<IdOperationResult> UpdateAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 删除实名认证信息。
    /// </summary>
    /// <param name="authentication">要删除的实名认证信息。</param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteAsync(RealNameAuthentication authentication);

    /// <summary>
    /// 删除关联特定自然人的所有实名认证信息。
    /// </summary>
    /// <param name="personId">关联的自然人的Id.</param>
    /// <returns></returns>
    Task<IdOperationResult> DeleteByPersonIdAsync(string personId);

    /// <summary>
    /// 根据自然人查找与之关联的实名认证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    IQueryable<RealNameAuthentication> FindByPerson(NaturalPerson person);

    /// <summary>
    /// 查找指定Id的实名认证信息。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<RealNameAuthentication?> FindByIdAsync(string id);
}
