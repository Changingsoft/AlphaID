using IDSubjects;
using IDSubjects.RealName;
using IDSubjectsTests.RealName;
using Microsoft.Extensions.DependencyInjection;

namespace IDSubjectsTests.Fixtures;
public class ServiceProviderFixture : IDisposable
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddIdentityCore<NaturalPerson>()
            .AddUserManager<NaturalPersonManager>()
            .AddUserStore<StubNaturalPersonStore>();

        services.AddScoped<ChineseIDCardManager>()
            .AddScoped<IChineseIDCardValidationStore, StubRealNameValidationStore>();

        this.RootServiceProvider = services.BuildServiceProvider();
        this.ServiceScopeFactory = this.RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public void Dispose()
    {
        
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
