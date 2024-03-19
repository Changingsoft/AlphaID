using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.Tests;
public class ServiceProviderFixture : IDisposable
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddIdSubjects()
            .AddPersonStore<StubNaturalPersonStore>()
            .AddPasswordHistoryStore<StubPasswordHistoryStore>();

        this.RootServiceProvider = services.BuildServiceProvider();
        this.ServiceScopeFactory = this.RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public void Dispose()
    {

    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
