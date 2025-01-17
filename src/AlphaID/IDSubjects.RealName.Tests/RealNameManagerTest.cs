using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RealNameManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly ApplicationUser _person = new("zhangsan", new HumanNameInfo("张小三"));

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
            new HumanNameInfo("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");

        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await personManager.CreateAsync(_person);

        //Test
        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager<ApplicationUser>>();
        IdOperationResult result = await realManager.AuthenticateAsync(_person, authentication);

        Assert.True(result.Succeeded);
        Assert.Equal("张三", _person.HumanName?.FullName);
    }

    [Fact(Skip = "阻止更改不是NaturalPersonManager的职责，应考虑在AlphaIdPlatform上进行测试。")]
    public async Task CannotChangeNameWhenRealNameAuthenticationExists()
    {
        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await personManager.CreateAsync(_person);

        ApplicationUser target = (await personManager.FindByIdAsync(_person.Id))!;

        var realManager = scope.ServiceProvider.GetRequiredService<RealNameManager<ApplicationUser>>();
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
            new HumanNameInfo("张三", "张", "三"),
            DateTimeOffset.UtcNow,
            "Test validator");
        await realManager.AuthenticateAsync(target, authentication);


        target.HumanName = new HumanNameInfo("张三三");
        IdentityResult result = await personManager.UpdateAsync(target);
        Assert.False(result.Succeeded);
    }
}