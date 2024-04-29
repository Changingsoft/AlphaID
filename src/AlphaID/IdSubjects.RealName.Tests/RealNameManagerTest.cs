using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RealNameManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly NaturalPerson _person = new("zhangsan", new PersonNameInfo("张小三"));

    [Fact]
    public async Task AddAuthentication()
    {
        var authentication = new DocumentedRealNameAuthentication(
            new ChineseIdCardDocument
            {
                Address = "Address",
                CardNumber = "370686193704095897",
                DateOfBirth = new DateOnly(1937, 4, 9),
                Ethnicity = "汉",
                IssueDate = new DateOnly(2000, 1, 1),
                Name = "张三",
                Issuer = "Issuer",
                Sex = Sex.Male
            },
            new PersonNameInfo("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");

        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await personManager.CreateAsync(_person);

        //Test
        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();
        IdOperationResult result = await realManager.AuthenticateAsync(_person, authentication);

        Assert.True(result.Succeeded);
        Assert.Equal("张三", _person.PersonName.FullName);
    }

    [Fact]
    public async Task CannotChangeNameWhenRealNameAuthenticationExists()
    {
        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        await personManager.CreateAsync(_person);

        NaturalPerson target = (await personManager.FindByIdAsync(_person.Id))!;

        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();
        var authentication = new DocumentedRealNameAuthentication(
            new ChineseIdCardDocument
            {
                Address = "Address",
                CardNumber = "370686193704095897",
                DateOfBirth = new DateOnly(1937, 4, 9),
                Ethnicity = "汉",
                IssueDate = new DateOnly(2000, 1, 1),
                Name = "张三",
                Issuer = "Issuer",
                Sex = Sex.Male
            },
            new PersonNameInfo("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");
        await realManager.AuthenticateAsync(target, authentication);


        target.PersonName = new PersonNameInfo("张三三");
        IdentityResult result = await personManager.UpdateAsync(target);
        Assert.False(result.Succeeded);
    }
}