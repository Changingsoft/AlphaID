using IdSubjects.Identity;
using IdSubjects.Tests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.Identity.Tests;

public class IdentityServiceProviderFixture
{
    public IdentityServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddIdSubjectsIdentity<ApplicationUser, IdentityRole>()
            .AddUserStore<StubApplicationUserStore>()
            .AddRoleStore<StubRoleStore>()
            .AddDefaultTokenProviders();
        services.AddScoped<ApplicationUserSignInManager<ApplicationUser>>();
        services.AddScoped<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
        services.AddScoped<IApplicationUserStore<ApplicationUser>, StubApplicationUserStore>();
        services.AddScoped<IPasswordHistoryStore, StubPasswordHistoryStore>();
        services.AddLogging();

        services.Configure<PasswordLifetimeOptions>(options =>
        {
            options.EnablePassExpires = true;
            options.RememberPasswordHistory = 1;
        });
        //注入一个假的HttpContext
        services.AddScoped<IHttpContextAccessor, MockHttpContextAccessor>();

        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }

}