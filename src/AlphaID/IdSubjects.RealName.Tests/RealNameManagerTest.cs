using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RealNameManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly NaturalPerson person = new("zhangsan", new PersonNameInfo("张小三"));

    [Fact]
    public async Task AddAuthentication()
    {
        var authentication = new DocumentedRealNameAuthentication(
            new ChineseIdCardDocument()
            {
                Address = "Address",
                CardNumber = "370686193704095897",
                DateOfBirth = new(1937, 4, 9),
                Ethnicity = "汉",
                IssueDate = new(2000, 1, 1),
                Name = "张三",
                Issuer = "Issuer",
                Sex = Sex.Male,
            },
            new("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");

        using var scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await personManager.CreateAsync(this.person);

        //Test
        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();
        var result = await realManager.AuthenticateAsync(this.person, authentication);

        Assert.True(result.Succeeded);
        Assert.Equal("张三", this.person.PersonName.FullName);
    }

    [Fact]
    public async Task CannotChangeNameWhenRealNameAuthenticationExists()
    {
        using var scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await personManager.CreateAsync(this.person);

        var target = (await personManager.FindByIdAsync(this.person.Id))!;

        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();
        var authentication = new DocumentedRealNameAuthentication(
            new ChineseIdCardDocument()
            {
                Address = "Address",
                CardNumber = "370686193704095897",
                DateOfBirth = new(1937, 4, 9),
                Ethnicity = "汉",
                IssueDate = new(2000, 1, 1),
                Name = "张三",
                Issuer = "Issuer",
                Sex = Sex.Male,
            },
            new("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");
        await realManager.AuthenticateAsync(target, authentication);


        target.PersonName = new("张三三");
        var result = await personManager.UpdateAsync(target);
        Assert.False(result.Succeeded);
    }
}
