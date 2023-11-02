using IDSubjects;
using IDSubjectsTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IDSubjectsTests;

[Collection(nameof(ServiceProviderCollection))]
public class PersonManagerTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;
    private readonly NaturalPersonMocker naturalPersonMocker = new();

    public PersonManagerTests(ServiceProviderFixture serviceProviderFixture)
    {
        this.serviceProviderFixture = serviceProviderFixture;
    }


    [Fact]
    public async Task ChangeName()
    {
        using var scope = this.serviceProviderFixture.ServiceScopeFactory.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var person = await this.naturalPersonMocker.CreateDefaultMockPersonAsync(manager);

        var chineseName = new ChinesePersonName("张", "三", "ZHANG", "SAN");

        var result = await manager.ChangeNameAsync(person, chineseName);
        Assert.True(result.Succeeded);
        Assert.Equal("张", person.LastName);
        Assert.Equal("三", person.FirstName);
        Assert.Equal("ZHANG", person.PhoneticSurname);
        Assert.Equal("SAN", person.PhoneticGivenName);
        Assert.Equal("ZHANGSAN", person.PhoneticSearchHint);
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
}