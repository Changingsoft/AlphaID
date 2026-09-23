using Duende.IdentityServer.Configuration;
using IntegrationTestUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace AuthCenterWebApp.Tests;

public class AuthCenterWebAppFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);
        //内置数据（IdentityServer 的客户端、API 范围、标识资源）是令牌端点与授权码流程的前提，
        //测试不应再依赖「数据库碰巧被 DatabaseTool 灌过」。这里只补齐缺失的部分，不建库、不删库。
        host.Services.EnsureInitData();
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
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