using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// </summary>
public abstract class ApplicationUserCreateInterceptor : IApplicationUserCreateInterceptor
{
    /// <summary>
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> PreCreateAsync(ApplicationUserManager<ApplicationUser> personManager,
        ApplicationUser person,
        string? password = null)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostCreateAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person)
    {
        return Task.CompletedTask;
    }
}