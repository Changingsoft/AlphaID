using IdSubjects.Tests.Fixtures;
using IntegrationTestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdSubjects.Tests;

public class NaturalPersonManagerTest : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture serviceProviderFixture;
    private readonly NaturalPersonMocker naturalPersonMocker = new();

    public NaturalPersonManagerTest(ServiceProviderFixture serviceProviderFixture)
    {
        this.serviceProviderFixture = serviceProviderFixture;
    }

    [Fact]
    public async Task SetTimeZone()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        var result = await manager.SetTimeZone(person, "Asia/Shanghai");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task WithPasswordWillSetPasswordLastSet()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();

        var person = new NaturalPerson("new-test-person", new PersonNameInfo("FullName", "Surname", "GivenName"));
        var result = await manager.CreateAsync(person, "password");
        Assert.True(person.PasswordLastSet.HasValue);
    }

    [Fact]
    public async Task KeepPasswordLastSetBeNullIfCreateWithoutPassword()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();

        var person = new NaturalPerson("new-test-person", new PersonNameInfo("FullName", "Surname", "GivenName"));
        var result = await manager.CreateAsync(person);
        Assert.False(person.PasswordLastSet.HasValue);
    }

    [Fact]
    public async Task SetTimesWhenCreatePerson()
    {
        var utcNow = new DateTimeOffset(2023, 11, 4, 3, 50, 34, TimeSpan.Zero);

        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        manager.TimeProvider = new StubTimeProvider(utcNow);

        var person = new NaturalPerson("new-test-person", new PersonNameInfo("FullName", "Surname", "GivenName"));
        var result = await manager.CreateAsync(person);

        Assert.Equal(utcNow, person.WhenCreated);
        Assert.Equal(utcNow, person.WhenChanged);
    }

    [Fact]
    public async Task SetUpdateTimeWhenUpdate()
    {
        var utcNow = new DateTimeOffset(2023, 11, 4, 3, 50, 34, TimeSpan.Zero);

        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        manager.TimeProvider = new StubTimeProvider(utcNow);

        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);
        await manager.UpdateAsync(person);

        Assert.Equal(utcNow, person.WhenChanged);
    }

    [Fact]
    public async Task AddPasswordWillSetPasswordLastSetTime()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        var result = await manager.AddPasswordAsync(person, "Password$1");
        Assert.True(result.Succeeded);
        Assert.NotNull(person.PasswordLastSet);
    }
}