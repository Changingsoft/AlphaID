using IdSubjects.RealName.Requesting;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RealNameRequestManagerTest(ServiceProviderFixture serviceProvider) : IClassFixture<ServiceProviderFixture>
{
    private readonly NaturalPerson person = new("zhangsan", new PersonNameInfo("张三"));
    private readonly RealNameRequest request = new ChineseIdCardRealNameRequest("张三", Sex.Male, "汉", new DateOnly(1990, 1, 1), "Address",
            "370686193704095897", "Issuer", new DateOnly(2000, 1, 1), new DateOnly(2020, 1, 1),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]));

    [Fact]
    public async Task AddRequest()
    {
        using var scope = serviceProvider.ScopeFactory.CreateScope();
        {
            var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
            var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager>();
            await personManager.CreateAsync(this.person);

            var result = await realnameRequestManager.CreateAsync(this.person, this.request);

            Assert.True(result.Succeeded);
            Assert.Equal(this.person.Id, this.request.PersonId);
            Assert.False(this.request.Accepted.HasValue);
        }
    }

    [Fact]
    public async Task AcceptRequest()
    {
        using var scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager>();
        var realnameManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();

        await personManager.CreateAsync(this.person);
        await realnameRequestManager.CreateAsync(this.person, this.request);
        var result = await realnameRequestManager.AcceptAsync(this.request, "Auditor");
        Assert.True(result.Succeeded);
        Assert.True(this.request.Accepted.HasValue);
        Assert.True(this.request.Accepted.Value);
        Assert.Equal("Auditor", this.request.Auditor);

        var authentications = realnameManager.GetAuthentications(this.person);
        var authentication = authentications.Single();
        Assert.True(authentication.Applied);
        Assert.Equal("张三", authentication.PersonName.FullName);
    }

    [Fact]
    public async Task RefuseRequest()
    {
        using var scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<NaturalPersonManager>();
        var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager>();
        var realnameManager = scope.ServiceProvider.GetRequiredService<RealNameManager>();

        await personManager.CreateAsync(this.person);
        await realnameRequestManager.CreateAsync(this.person, this.request);
        var result = await realnameRequestManager.RefuseAsync(this.request, "Auditor");
        Assert.True(result.Succeeded);
        Assert.True(this.request.Accepted.HasValue);
        Assert.False(this.request.Accepted.Value);
        Assert.Equal("Auditor", this.request.Auditor);
    }
}
