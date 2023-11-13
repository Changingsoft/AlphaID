using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlphaIDWebAPITests;

public class AlphaIdApiFactory : WebApplicationFactory<AlphaIDWebAPI.Program>
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
                //Replace AuthenticationHandler of Bearer scheme for testing.
                options.Schemes.First(s => s.Name == JwtBearerDefaults.AuthenticationScheme).HandlerType = typeof(TestAuthHandler);
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
        HttpClient client = options != null ? this.CreateClient(options) : this.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme);
        return client;
    }
}
