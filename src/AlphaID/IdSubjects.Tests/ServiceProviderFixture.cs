using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.Tests;

public class ServiceProviderFixture : IDisposable
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddIdSubjectsIdentity<ApplicationUser, IdentityRole>()
            .AddUserStore<StubApplicationUserStore>()
            .AddRoleStore<StubRoleStore>()
            .AddDefaultTokenProviders();
        services.AddIdSubjects<ApplicationUser>()
            .AddPersonStore<StubApplicationUserStore, ApplicationUser>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        services.AddScoped<ApplicationUserSignInManager<ApplicationUser>>();
        services.AddScoped<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
        services.Configure<PasswordLifetimeOptions>(options =>
        {
            options.EnablePassExpires = true;
            options.RememberPasswordHistory = 1;
        });
        //注入一个假的HttpContext
        services.AddScoped<IHttpContextAccessor, MockHttpContextAccessor>();

        services.AddAuthentication()
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }

    public void Dispose()
    {
    }
}