using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace AlphaIdWebAPI.Tests;

public class AlphaIdApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                //Replace AuthenticationHandler of Bearer scheme for testing.
                options.Schemes.First(s => s.Name == JwtBearerDefaults.AuthenticationScheme).HandlerType =
                    typeof(TestAuthHandler);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

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
        HttpClient client = options != null ? CreateClient(options) : CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme);
        return client;
    }
}