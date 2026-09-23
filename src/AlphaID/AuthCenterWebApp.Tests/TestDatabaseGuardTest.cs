using AlphaId.EntityFramework;
using AlphaId.EntityFramework.IdSubjects;
using IntegrationTestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenterWebApp.Tests;

/// <summary>
/// 守卫测试：证明这套集成测试连的确实是测试自己建的库，而不是开发库。
/// </summary>
/// <remarks>
/// 配置覆盖一旦失效（比如换了配置源顺序、或某个 DbContext 走了别的连接字符串键），
/// 测试会**静默地**退回开发库——本机看起来全绿，别人机器上却莫名其妙地红。这条测试就是拦住这件事的。
/// </remarks>
[Collection(nameof(TestServerCollection))]
public class TestDatabaseGuardTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public void HostUsesSelfProvisionedDatabase()
    {
        IConfiguration configuration = factory.Services.GetRequiredService<IConfiguration>();
        string? configured = configuration.GetConnectionString("DefaultConnection");

        Assert.NotNull(configured);
        Assert.StartsWith("AlphaIdTest-", factory.Database.Name);
        Assert.Contains(factory.Database.Name, configured);

        // 整份配置里都不允许再出现开发库的痕迹：任何一处漏改都会让某个功能悄悄连回开发库。
        foreach (KeyValuePair<string, string?> entry in configuration.AsEnumerable())
        {
            Assert.DoesNotContain("AlphaIDData-", entry.Value ?? string.Empty);
        }

        // 配置覆盖要一路生效到 EF：从容器里取出真正被应用使用的 DbContext，核对它实际连的库。
        using IServiceScope scope = factory.Services.CreateScope();
        AlphaIdDbContext platform = scope.ServiceProvider.GetRequiredService<AlphaIdDbContext>();
        Assert.Contains(factory.Database.Name, platform.Database.GetConnectionString()!);

        AlphaIdIdentityDbContext identity = scope.ServiceProvider.GetRequiredService<AlphaIdIdentityDbContext>();
        Assert.Contains(factory.Database.Name, identity.Database.GetConnectionString()!);
    }
}
