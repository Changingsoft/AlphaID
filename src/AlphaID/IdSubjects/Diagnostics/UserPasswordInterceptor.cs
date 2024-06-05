using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

/// <summary>
///     一个密码操作拦截器的空实现。
/// </summary>
public class UserPasswordInterceptor : IUserPasswordInterceptor
{
    /// <summary>
    /// </summary>
    /// <param name="person"></param>
    /// <param name="plainPassword"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public virtual Task<IdentityResult> PasswordChangingAsync(NaturalPerson person,
        string? plainPassword,
        CancellationToken cancellation)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// </summary>
    /// <param name="person"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public virtual Task PasswordChangedAsync(NaturalPerson person, CancellationToken cancellation)
    {
        return Task.CompletedTask;
    }
}