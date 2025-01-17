using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// </summary>
public interface IApplicationUserDeleteInterceptor : IInterceptor
{
    /// <summary>
    ///     在删除自然人之前调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<IdentityResult> PreDeleteAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person);

    /// <summary>
    ///     在删除自然人之后调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostDeleteAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person);
}