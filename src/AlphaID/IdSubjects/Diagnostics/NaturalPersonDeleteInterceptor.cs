using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// 
/// </summary>
public abstract class NaturalPersonDeleteInterceptor : INaturalPersonDeleteInterceptor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> PreDeleteAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostDeleteAsync(NaturalPersonManager personManager, NaturalPerson person)
    {
        return Task.CompletedTask;
    }
}