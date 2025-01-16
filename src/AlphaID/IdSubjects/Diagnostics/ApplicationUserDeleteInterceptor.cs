using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// </summary>
public abstract class ApplicationUserDeleteInterceptor : IApplicationUserDeleteInterceptor
{
    /// <summary>
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> PreDeleteAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostDeleteAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person)
    {
        return Task.CompletedTask;
    }
}