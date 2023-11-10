using IDSubjects.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.QualityTools.Testing.Fakes;
using Xunit;

namespace IDSubjects.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class NaturalPersonManagerTest
{
    private readonly ServiceProviderFixture serviceProviderFixture;
    private readonly NaturalPersonMocker naturalPersonMocker = new();

    public NaturalPersonManagerTest(ServiceProviderFixture serviceProviderFixture)
    {
        this.serviceProviderFixture = serviceProviderFixture;
    }


    [Fact]
    public async Task ChangeNameWhenCanEditNameFlagNotSet()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        var personName = new PersonNameInfo("张三", "张", "三");

        var result = await manager.ChangePersonNameAsync(person, personName);
        Assert.False(result.Succeeded);
        Assert.NotEqual(personName, person.PersonName);
        //Assert.Equal(personName, person.PersonName);
    }

    [Fact]
    public async Task ChangeNameWhenCanEditNameFlagSet()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        var personName = new PersonNameInfo("张三", "张", "三");
        person.CanEditPersonName = true;
        var result = await manager.ChangePersonNameAsync(person, personName);
        Assert.True(result.Succeeded);
        Assert.Equal(personName, person.PersonName);
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
        using (ShimsContext.Create())
        {
            var utcNow = new DateTime(2023, 11, 4, 3, 50, 34, DateTimeKind.Utc);
            System.Fakes.ShimDateTime.UtcNowGet = () => utcNow;

            using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();

            var person = new NaturalPerson("new-test-person", new PersonNameInfo("FullName", "Surname", "GivenName"));
            var result = await manager.CreateAsync(person);
            Assert.Equal(utcNow, person.WhenCreated);
            Assert.Equal(utcNow, person.WhenChanged);
        }
    }
    [Fact]
    public async Task SetUpdateTimeWhenUpdate()
    {

        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        using (ShimsContext.Create())
        {
            var utcNow = new DateTime(2023, 11, 4, 3, 50, 34, DateTimeKind.Utc);
            System.Fakes.ShimDateTime.UtcNowGet = () => utcNow;

            await manager.UpdateAsync(person);

            Assert.Equal(utcNow, person.WhenChanged);
            Assert.NotEqual(utcNow, person.WhenCreated);
        }
    }

    [Fact]
    public async Task AddPasswordWillSetPasswordLastSetTime()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        using (ShimsContext.Create())
        {
            var utcNow = new DateTime(2023, 11, 4, 4, 50, 34, DateTimeKind.Utc);
            System.Fakes.ShimDateTime.UtcNowGet = () => utcNow;

            var result = await manager.AddPasswordAsync(person, "Password$1");
            Assert.True(result.Succeeded);
            Assert.Equal(utcNow, person.PasswordLastSet);
        }
    }
}