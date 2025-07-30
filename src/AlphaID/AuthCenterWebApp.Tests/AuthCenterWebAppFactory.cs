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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            //    .AddCookie()
            //    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { })
            //    .AddScheme<AuthenticationSchemeOptions, BearerTestAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, null);
            services.AddSingleton<IAuthenticationHandlerProvider, TestServerAuthenticationHandlerProvider>();
            services.AddSingleton<TestAuthHandler>();
            services.AddSingleton<BearerTestAuthenticationHandler>();
        });
    }

    public virtual HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        return client;
    }
    public virtual HttpClient CreateBearerTokenClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, "<Bearer Token>");
        return client;
    }
}