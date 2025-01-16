using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
///     提供一个具有默认行为的拦截器。如果你不想实现所有拦截方法，可以继承此拦截器然后重写所需的方法。
/// </summary>
public abstract class ApplicationUserUpdateInterceptor : IApplicationUserUpdateInterceptor
{
    /// <summary>
    ///     在更新ApplicationUser前调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns>始终返回表示成功的IdentityResult。</returns>
    public virtual Task<IdentityResult> PreUpdateAsync(ApplicationUserManager personManager, ApplicationUser person)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public virtual Task PostUpdateAsync(ApplicationUserManager personManager, ApplicationUser person)
    {
        return Task.CompletedTask;
    }
}