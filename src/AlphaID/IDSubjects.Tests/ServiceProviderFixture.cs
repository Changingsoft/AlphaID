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

        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public void Dispose()
    {

    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
