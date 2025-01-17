using Microsoft.AspNetCore.Identity;

namespace IdSubjects.RealName;

/// <summary>
///     实名认证管理器。
/// </summary>
/// <remarks>
///     初始化实名认证管理器。
/// </remarks>
/// <param name="store"></param>
/// <param name="applicationUserManager"></param>
public class RealNameManager<T>(IRealNameAuthenticationStore store, UserManager<T> applicationUserManager)
where T : ApplicationUser
{
    /// <summary>
    ///     获取可查询的实名认证信息集合。
    /// </summary>
    public IQueryable<RealNameAuthentication> Authentications => store.Authentications;


    /// <summary>
    ///     获取与自然人相关的实名状态信息。
    /// </summary>
    /// <param name="person"></param>
    /// <returns>与自然人相关的实名状态。如果没有，则返回null。</returns>
    public virtual IEnumerable<RealNameAuthentication> GetAuthentications(T person)
    {
        return store.FindByPerson(person);
    }

    /// <summary>
    ///     向指定的自然人添加实名认证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="authentication"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AuthenticateAsync(T person, RealNameAuthentication authentication)
    {
        authentication.PersonId = person.Id;
        IdOperationResult result = await store.CreateAsync(authentication);
        if (!result.Succeeded)
            return result;

        //为 person 应用更改。
        person.HumanName = authentication.PersonName;
        IdentityResult identityResult = await applicationUserManager.UpdateAsync(person);
        if (!identityResult.Succeeded)
            return IdOperationResult.Failed(identityResult.Errors.Select(e => e.Description).ToArray());

        return IdOperationResult.Success;
    }

    /// <summary>
    /// 获取一个值，确认某个自然人是否已通过实名认证。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public bool IsAuthenticated(string personId)
    {
        return store.Authentications.Any(a => a.PersonId == personId);
    }

    /// <summary>
    /// 重置自然人的验证状态。此方法将删除与特定自然人关联的所有验证结果。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public async Task ResetAsync(string personId)
    {
        await store.DeleteByPersonIdAsync(personId);
    }

    /// <summary>
    ///     查找指定的实名认证信息。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<RealNameAuthentication?> FindByIdAsync(string id)
    {
        return store.FindByIdAsync(id);
    }
}