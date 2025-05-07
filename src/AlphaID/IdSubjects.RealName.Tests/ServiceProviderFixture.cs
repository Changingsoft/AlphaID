using IdSubjects.DependencyInjection;
using IdSubjects.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        IdSubjectsBuilder idSubjectsBuilder = services.AddIdSubjects<ApplicationUser>()
            .AddPersonStore<StubApplicationUserStore, ApplicationUser>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        idSubjectsBuilder.AddRealName<ApplicationUser>()
            .AddRealNameAuthenticationStore<StubRealNameAuthenticationStore>()
            .AddRealNameRequestStore<StubRealNameRequestStore>();
        idSubjectsBuilder.IdentityBuilder.AddDefaultTokenProviders();

        Root = services.BuildServiceProvider();
        ScopeFactory = Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}