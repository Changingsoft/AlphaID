using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// 
/// </summary>
public abstract class NaturalPersonCreateInterceptor : INaturalPersonCreateInterceptor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> PreCreateAsync(NaturalPersonManager personManager, NaturalPerson person,
        string? password = null)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostCreateAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }
}