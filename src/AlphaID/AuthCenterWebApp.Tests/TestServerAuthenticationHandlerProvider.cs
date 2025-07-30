using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenterWebApp.Tests;

internal class TestServerAuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    public Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
        if (authenticationScheme.Equals(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<IAuthenticationHandler?>(context.RequestServices.GetRequiredService<BearerTestAuthenticationHandler>());
        }

        if (authenticationScheme.Equals(CookieAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<IAuthenticationHandler?>(context.RequestServices.GetRequiredService<TestAuthHandler>());
        }

        return Task.FromResult<IAuthenticationHandler?>(null);
    }
}
