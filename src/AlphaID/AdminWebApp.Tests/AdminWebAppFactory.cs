using IntegrationTestUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace AdminWebApp.Tests;

public class AdminWebAppFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// 本测试项目独占的测试数据库。测试自己建库、跑迁移、灌数据，不依赖开发库，也不需要先跑 DatabaseTool。
    /// </summary>
    public TestDatabase Database { get; } = TestDatabase.For(typeof(AdminWebAppFactory).Assembly);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");

        // 保留 Development 环境，但把数据库整体换成测试自建库，否则会读到 appsettings.Development.json 指向的开发库。
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(Database.ConnectionStringOverrides()));

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "TestScheme";
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
        });
    }

    public virtual HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        return client;
    }
}
