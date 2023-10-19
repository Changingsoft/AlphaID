using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlphaIDWebAPITests;

public class AlphaIDAPIFactory : WebApplicationFactory<AlphaIDWebAPI.Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            //services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            //{
            //    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>("https://localhost:49726/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever(), AuthCenterWebAppFactory.Instance.CreateClient());
            //});
        });
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "TestBearer";
                options.DefaultAuthenticateScheme = "TestBearer";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestBearer", options => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("TestBearer")
                .RequireClaim("scope", "openid")
                .Build();

                options.AddPolicy("RealNameScopeRequired", builder =>
                {
                    builder.AuthenticationSchemes.Add("TestBearer");
                    builder.RequireClaim("scope", "realname");
                });
            });
        });

    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        //#if DEBUG
        //        var workDir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\DatabaseTool\bin\Debug\net7.0\";
        //        var process = new Process();
        //        process.StartInfo.FileName = workDir + @"DatabaseTool.exe";
        //        process.StartInfo.WorkingDirectory = workDir;
        //        process.Start();
        //        process.WaitForExit();
        //#endif
        //#if RELEASE
        //        var workDir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\DatabaseTool\bin\Release\net7.0\";
        //        var process = new Process();
        //        process.StartInfo.FileName = workDir + @"DatabaseTool.exe";
        //        process.StartInfo.WorkingDirectory = workDir;
        //        process.Start();
        //        process.WaitForExit();
        //#endif
        return host;
    }

    public virtual HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client;
        if (options != null)
            client = this.CreateClient(options);
        else
            client = this.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestBearer");
        return client;
    }
}
