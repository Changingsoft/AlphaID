using IdSubjects.DependencyInjection;
using IdSubjects.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        IdSubjectsBuilder idSubjectsBuilder = services.AddIdSubjects()
            .AddPersonStore<StubApplicationUserStore, ApplicationUser>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        idSubjectsBuilder.AddRealName()
            .AddRealNameAuthenticationStore<StubRealNameAuthenticationStore>()
            .AddRealNameRequestStore<StubRealNameRequestStore>();

        Root = services.BuildServiceProvider();
        ScopeFactory = Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}