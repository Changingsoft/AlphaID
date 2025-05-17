using IdSubjects.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        IdentityBuilder idSubjectsBuilder = services.AddIdSubjects<ApplicationUser>()
            .AddUserStore<StubApplicationUserStore>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        idSubjectsBuilder.AddRealName<ApplicationUser>()
            .AddRealNameAuthenticationStore<StubRealNameAuthenticationStore>()
            .AddRealNameRequestStore<StubRealNameRequestStore>();
        //idSubjectsBuilder.AddDefaultTokenProviders();

        Root = services.BuildServiceProvider();
        ScopeFactory = Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}