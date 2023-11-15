namespace IdSubjects.Diagnostics;

/// <summary>
/// 一个密码操作拦截器的空实现。
/// </summary>
public class PasswordInterceptor : IPasswordInterceptor
{
    /// <inheritdoc />
    public virtual Task PasswordChangingAsync(NaturalPerson person, string plainPassword,
        CancellationToken cancellation)
    {
        return Task.CompletedTask;
    }

    
    /// <inheritdoc />
    public virtual Task PasswordChangedAsync(NaturalPerson person, string plainPassword, CancellationToken cancellation)
    {
        return Task.CompletedTask;
    }
}