using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.Tests;

public class ServiceProviderFixture : IDisposable
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        var builder = services.AddIdSubjects<ApplicationUser>()
            .AddUserStore<StubApplicationUserStore>()
            .AddDefaultTokenProviders();
        services.AddDataProtection();
        services.AddScoped<IPasswordHistoryStore, StubPasswordHistoryStore>();
        services.AddScoped<IApplicationUserStore<ApplicationUser>, StubApplicationUserStore>();

        services.AddScoped<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
        services.Configure<PasswordLifetimeOptions>(options =>
        {
            options.EnablePassExpires = true;
            options.RememberPasswordHistory = 1;
        });
        //注入一个假的HttpContext

        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }

    public void Dispose()
    {
    }
}