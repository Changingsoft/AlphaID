using Microsoft.AspNetCore.Identity;

namespace IdSubjects.RealName;

/// <summary>
///     实名认证管理器。
/// </summary>
/// <remarks>
///     初始化实名认证管理器。
/// </remarks>
/// <param name="store"></param>
/// <param name="naturalPersonManager"></param>
public class RealNameManager(IRealNameAuthenticationStore store, NaturalPersonManager naturalPersonManager)
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
    public virtual IEnumerable<RealNameAuthentication> GetAuthentications(NaturalPerson person)
    {
        return store.FindByPerson(person);
    }

    /// <summary>
    ///     向指定的自然人添加实名认证信息。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="authentication"></param>
    /// <returns></returns>
    public async Task<IdOperationResult> AuthenticateAsync(NaturalPerson person, RealNameAuthentication authentication)
    {
        authentication.PersonId = person.Id;
        IdOperationResult result = await store.CreateAsync(authentication);
        if (!result.Succeeded)
            return result;

        //为 person 应用更改。
        IdentityResult identityResult = await naturalPersonManager.UpdateAsync(person);
        if (!identityResult.Succeeded)
            return IdOperationResult.Failed(identityResult.Errors.Select(e => e.Description).ToArray());

        return IdOperationResult.Success;
    }

    /// <summary>
    ///     删除
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns></returns>
    public Task<IdOperationResult> RemoveAsync(RealNameAuthentication authentication)
    {
        return store.DeleteAsync(authentication);
    }

    internal Task<IdOperationResult> UpdateAsync(RealNameAuthentication authentication)
    {
        return store.UpdateAsync(authentication);
    }

    internal bool HasAuthenticated(NaturalPerson person)
    {
        return store.Authentications.Any(a => a.PersonId == person.Id);
    }

    internal IEnumerable<RealNameAuthentication> GetPendingAuthentications(NaturalPerson person)
    {
        return store.FindByPerson(person).Where(a => !a.Applied);
    }

    internal async Task ClearAsync(NaturalPerson person)
    {
        await store.DeleteByPersonIdAsync(person.Id);
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