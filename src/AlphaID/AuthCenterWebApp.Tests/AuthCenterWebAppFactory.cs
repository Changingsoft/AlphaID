using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthCenterWebApp.Tests;

public class AuthCenterWebAppFactory : WebApplicationFactory<Program>
{
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
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(CookieAuthenticationDefaults.AuthenticationScheme, null);

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