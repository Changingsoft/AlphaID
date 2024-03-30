using Microsoft.AspNetCore.Identity;

namespace IdSubjects.Diagnostics;
internal class NaturalPersonUpdateInterceptorAggregator(IEnumerable<INaturalPersonUpdateInterceptor> interceptors)
{
    private readonly Stack<INaturalPersonUpdateInterceptor> _stack = new();

    public async Task<IdentityResult> PreUpdateAsync(NaturalPersonManager manager, NaturalPerson person)
    {
        bool success = true;
        List<IdentityError> errors = [];
        foreach (var interceptor in interceptors)
        {
            _stack.Push(interceptor);
            var interceptorResult = await interceptor.PreUpdateAsync(manager, person);
            if (!interceptorResult.Succeeded)
                success = false;
            errors.AddRange(interceptorResult.Errors);
        }

        return success ? IdentityResult.Success : IdentityResult.Failed([.. errors]);
    }

    public async Task PostUpdateAsync(NaturalPersonManager manager, NaturalPerson person)
    {
        while (_stack.TryPop(out var interceptor))
        {
            await interceptor.PostUpdateAsync(manager, person);
        }
    }
}
