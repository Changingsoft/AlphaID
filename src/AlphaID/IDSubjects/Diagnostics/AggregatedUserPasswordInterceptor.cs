using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

internal class AggregatedUserPasswordInterceptor(IEnumerable<IUserPasswordInterceptor> interceptors)
    : IUserPasswordInterceptor
{
    private readonly Stack<IUserPasswordInterceptor> _stack = new();

    public async Task<IdentityResult> PasswordChangingAsync(NaturalPerson person,
        string? plainPassword,
        CancellationToken cancellation)
    {
        List<IdentityError> errors = [];
        var success = true;
        foreach (IUserPasswordInterceptor interceptor in interceptors)
        {
            _stack.Push(interceptor);
            IdentityResult result = await interceptor.PasswordChangingAsync(person, plainPassword, cancellation);
            if (!result.Succeeded)
                success = false;
            errors.AddRange(result.Errors);
        }

        return success ? IdentityResult.Success : IdentityResult.Failed([.. errors]);
    }

    public async Task PasswordChangedAsync(NaturalPerson person, CancellationToken cancellation)
    {
        while (_stack.TryPop(out IUserPasswordInterceptor? interceptor))
            await interceptor.PasswordChangedAsync(person, cancellation);
    }
}