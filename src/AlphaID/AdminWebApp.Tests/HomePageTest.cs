using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace AdminWebApp.Tests;

[Collection(nameof(TestServerCollection))]
public class HomePageTest(AdminWebAppFactory factory)
{
    [Fact]
    public async Task UnauthenticatedUserRedirect()
    {
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        HttpResponseMessage response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedUserOk()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
    }
}