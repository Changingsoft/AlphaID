using Duende.IdentityServer.Configuration;
using IntegrationTestUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace AuthCenterWebApp.Tests;

public class AuthCenterWebAppFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// 本测试项目独占的测试数据库。测试自己建库、跑迁移、灌数据，不依赖开发库，也不需要先跑 DatabaseTool。
    /// </summary>
    public TestDatabase Database { get; } = TestDatabase.For(typeof(AuthCenterWebAppFactory).Assembly);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // 保留 Development 环境（NopEmailSender、验证码放宽、令牌清理关闭等都依赖它），
        // 但把数据库整体换成测试自建库，否则会读到 appsettings.Development.json 指向的开发库。
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(Database.ConnectionStringOverrides()));

        builder.ConfigureTestServices(services =>
        {
            services.Configure<IdentityServerOptions>(options =>
            {
                //hack: 修正自动测试阶段返回地址参数名选项为null的问题。
                options.UserInteraction.LoginReturnUrlParameter = "returnUrl";
            });

            //添加Cookies认证方案
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, CookiesTestAuthenticationHandler>(CookieAuthenticationDefaults.AuthenticationScheme, null);

            //替换JwtBearer默认处理器
            services.PostConfigure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
            {
                var bearerScheme = options.Schemes.FirstOrDefault(s => s.Name == JwtBearerDefaults.AuthenticationScheme);
                if (bearerScheme != null)
                {
                    bearerScheme.HandlerType = typeof(BearerTestAuthenticationHandler);
                }
            });
        });
    }

    public virtual HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(CookieAuthenticationDefaults.AuthenticationScheme, "<Cookie Token>");
        return client;
    }
    public virtual HttpClient CreateBearerTokenClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, "<Bearer Token>");
        return client;
    }
}
