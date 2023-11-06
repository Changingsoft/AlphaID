using Microsoft.Extensions.DependencyInjection;

namespace IDSubjects.Tests.Fixtures;
public class ServiceProviderFixture : IDisposable
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddIdentityCore<NaturalPerson>()
            .AddUserManager<NaturalPersonManager>()
            .AddUserStore<StubNaturalPersonStore>()
            .AddErrorDescriber<NaturalPersonIdentityErrorDescriber>();
        services.AddScoped<NaturalPersonIdentityErrorDescriber>();

        this.RootServiceProvider = services.BuildServiceProvider();
        this.ServiceScopeFactory = this.RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public void Dispose()
    {

    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
