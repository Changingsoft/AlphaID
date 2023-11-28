using IdSubjects.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;
public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        var idSubjectsBuilder = services.AddIdSubjects()
            .AddPersonStore<StubNaturalPersonStore>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        idSubjectsBuilder.AddRealName()
            .AddRealNameAuthenticationStore<StubRealNameAuthenticationStore>()
            .AddRealNameRequestStore<StubRealNameRequestStore>();

        this.Root = services.BuildServiceProvider();
        this.ScopeFactory = this.Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}
