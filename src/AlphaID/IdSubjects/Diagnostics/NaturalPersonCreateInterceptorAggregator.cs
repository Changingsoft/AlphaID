using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;

internal class NaturalPersonCreateInterceptorAggregator(IEnumerable<INaturalPersonCreateInterceptor> interceptors)
{
    private readonly Stack<INaturalPersonCreateInterceptor> _stack = new();

    public async Task<IdentityResult> PreCreate(NaturalPersonManager manager,
        NaturalPerson person,
        string? password = null)
    {
        List<IdentityError> errors = [];
        var success = true;
        foreach (INaturalPersonCreateInterceptor interceptor in interceptors)
        {
            _stack.Push(interceptor);
            IdentityResult result = await interceptor.PreCreateAsync(manager, person, password);
            if (!result.Succeeded)
                success = false;
            errors.AddRange(result.Errors);
        }

        return success ? IdentityResult.Success : IdentityResult.Failed([.. errors]);
    }

    public async Task PostCreate(NaturalPersonManager manager, NaturalPerson person)
    {
        while (_stack.TryPop(out INaturalPersonCreateInterceptor? interceptor))
            await interceptor.PostCreateAsync(manager, person);
    }
}