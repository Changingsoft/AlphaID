using IdSubjects.RealName.Requesting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdSubjects.RealName.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RealNameRequestManagerTest(ServiceProviderFixture serviceProvider)
{
    private readonly ApplicationUser _person = new("zhangsan", new HumanNameInfo("张三"));

    [Fact]
    public async Task AddRequest()
    {
        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager<ApplicationUser>>();
        await personManager.CreateAsync(_person);
        RealNameRequest request = new ChineseIdCardRealNameRequest(_person.Id, "张三", Sex.Male, "汉",
            new DateOnly(1990, 1, 1), "Address",
            "370686193704095897", "Issuer", new DateOnly(2000, 1, 1), new DateOnly(2020, 1, 1),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]));
        IdOperationResult result = await realnameRequestManager.CreateAsync(request);

        Assert.True(result.Succeeded);
        Assert.Equal(_person.Id, request.PersonId);
        Assert.False(request.Accepted.HasValue);
    }

    [Fact]
    public async Task AcceptRequest()
    {
        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager<ApplicationUser>>();
        var realnameManager = scope.ServiceProvider.GetRequiredService<RealNameManager<ApplicationUser>>();

        await personManager.CreateAsync(_person);
        RealNameRequest request = new ChineseIdCardRealNameRequest(_person.Id, "张三", Sex.Male, "汉",
            new DateOnly(1990, 1, 1), "Address",
            "370686193704095897", "Issuer", new DateOnly(2000, 1, 1), new DateOnly(2020, 1, 1),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]));
        await realnameRequestManager.CreateAsync(request);
        IdOperationResult result = await realnameRequestManager.AcceptAsync(request, "Auditor");
        Assert.True(result.Succeeded);
        Assert.True(request.Accepted.HasValue);
        Assert.True(request.Accepted.Value);
        Assert.Equal("Auditor", request.Auditor);

        IEnumerable<RealNameAuthentication> authentications = realnameManager.GetAuthentications(_person);
        RealNameAuthentication authentication = authentications.Single();
        Assert.Equal("张三", authentication.PersonName.FullName);
    }

    [Fact]
    public async Task RefuseRequest()
    {
        using IServiceScope scope = serviceProvider.ScopeFactory.CreateScope();
        var personManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var realnameRequestManager = scope.ServiceProvider.GetRequiredService<RealNameRequestManager<ApplicationUser>>();
        scope.ServiceProvider.GetRequiredService<RealNameManager<ApplicationUser>>();

        await personManager.CreateAsync(_person);
        RealNameRequest request = new ChineseIdCardRealNameRequest(_person.Id, "张三", Sex.Male, "汉",
            new DateOnly(1990, 1, 1), "Address",
            "370686193704095897", "Issuer", new DateOnly(2000, 1, 1), new DateOnly(2020, 1, 1),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]),
            new BinaryDataInfo("image/jpg", [0xff, 0xfe]));
        await realnameRequestManager.CreateAsync(request);
        IdOperationResult result = await realnameRequestManager.RefuseAsync(request, "Auditor");
        Assert.True(result.Succeeded);
        Assert.True(request.Accepted.HasValue);
        Assert.False(request.Accepted.Value);
        Assert.Equal("Auditor", request.Auditor);
    }
}