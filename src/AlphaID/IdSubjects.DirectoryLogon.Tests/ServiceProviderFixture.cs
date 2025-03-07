using IdSubjects.DependencyInjection;
using IdSubjects.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.DirectoryLogon.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        IdSubjectsBuilder idSubjectsBuilder = services.AddIdSubjects<ApplicationUser>()
            .AddPersonStore<StubApplicationUserStore, ApplicationUser>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();
        idSubjectsBuilder.AddDirectoryLogin<ApplicationUser>()
            .AddDirectoryServiceStore<StubDirectoryServiceDescriptorStore>()
            .AddLogonAccountStore<StubDirectoryAccountStore>();

        Root = services.BuildServiceProvider();
        ScopeFactory = Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}