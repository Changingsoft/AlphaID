using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace AuthCenterWebAppTests;

public class AuthCenterWebAppFactory : WebApplicationFactory<AuthCenterWebApp.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultScheme = "TestScheme";
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        });
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        var server = base.CreateServer(builder);
        //if(!DatabaseReady)
        //{
        //    lock(_lock)
        //    {
        //        if(!DatabaseReady)
        //        {
        //            using var scope = server.Services.CreateAsyncScope();
        //            var idDb = scope.ServiceProvider.GetRequiredService<IDSubjectsDbContext>();
        //            idDb.Database.EnsureDeleted();
        //            idDb.Database.Migrate();
        //            scope.Dispose();
        //            DatabaseReady = true;
        //        }
        //    }
        //}
        return server;
    }

    public virtual HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        HttpClient client = options != null ? this.CreateClient(options) : this.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        return client;
    }

}
