using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
///     自然人拦截器接口。
/// </summary>
public interface IApplicationUserUpdateInterceptor : IInterceptor
{
    /// <summary>
    ///     在更新自然人之前调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns>返回一个IdentityResult。如果IdentityResult.Succeeded指示true，则继续执行后续操作。否则用户管理器中止更新操作并汇总错误消息后返回。</returns>
    Task<IdentityResult> PreUpdateAsync(ApplicationUserManager personManager, ApplicationUser person);

    /// <summary>
    ///     在更新自然人之后调用。
    /// </summary>
    /// <param name="personManager"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    Task PostUpdateAsync(ApplicationUserManager personManager, ApplicationUser person);
}