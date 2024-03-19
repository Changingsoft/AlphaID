using System.Net;

namespace AdminWebApp.Tests;

[Collection(nameof(TestServerCollection))]
public class HomePageTest(AdminWebAppFactory factory)
{
    [Fact]
    public async Task UnauthenticatedUserRedirect()
    {
        var client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false,
        });
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedUserOk()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
    }
}