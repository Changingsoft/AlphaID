using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
/// 用于拦截密码操作的拦截器接口。
/// </summary>
/// <remarks>
/// <para>
/// 如果你不计划实现所有方法，请考虑实现<see cref="UserPasswordInterceptor"/>。
/// </para>
/// </remarks>
public interface IUserPasswordInterceptor : IInterceptor
{
    /// <summary>
    /// 在执行密码修改前调用。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="plainPassword"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task<IdentityResult> PasswordChangingAsync(NaturalPerson person, string plainPassword, CancellationToken cancellation);

    /// <summary>
    /// 在执行密码修改后调用。
    /// </summary>
    /// <param name="person"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task PasswordChangedAsync(NaturalPerson person, CancellationToken cancellation);
}