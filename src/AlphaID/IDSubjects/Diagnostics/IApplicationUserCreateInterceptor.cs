using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// </summary>
public interface IApplicationUserCreateInterceptor : IInterceptor
{
    /// <summary>
    ///     在创建自然人之前调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<IdentityResult> PreCreateAsync(ApplicationUserManager<ApplicationUser> personManager,
        ApplicationUser person,
        string? password = null);

    /// <summary>
    ///     在创建了自然人之后调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostCreateAsync(ApplicationUserManager<ApplicationUser> personManager, ApplicationUser person);
}