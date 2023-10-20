using System.Net;

namespace AdminWebAppTests;

public class HomePageTest : IClassFixture<AdminWebAppFactory>
{
    private readonly AdminWebAppFactory factory;

    public HomePageTest(AdminWebAppFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task UnauthenticatedUserRedirect()
    {
        var client = this.factory.CreateClient(new()
        {
            AllowAutoRedirect = false,
        });
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedUserOK()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
    }
}