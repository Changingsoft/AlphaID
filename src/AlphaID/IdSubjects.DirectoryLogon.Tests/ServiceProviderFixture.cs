using IdSubjects.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.DirectoryLogon.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();
        IdentityBuilder idSubjectsBuilder = services.AddIdSubjects<ApplicationUser>()
            .AddUserStore<StubApplicationUserStore>();
        idSubjectsBuilder.AddDirectoryLogin<ApplicationUser>()
            .AddDirectoryServiceStore<StubDirectoryServiceDescriptorStore>()
            .AddLogonAccountStore<StubDirectoryAccountStore>();

        Root = services.BuildServiceProvider();
        ScopeFactory = Root.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider Root { get; }

    public IServiceScopeFactory ScopeFactory { get; }
}