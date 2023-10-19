using System.Net;

namespace AuthCenterWebAppTests;
public class HomePageTest : IClassFixture<AuthCenterWebAppFactory>
{
    private readonly AuthCenterWebAppFactory factory;

    public HomePageTest(AuthCenterWebAppFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    public async Task ShouldBeOK(string url)
    {
        var client = this.factory.CreateAuthenticatedClient();
        var response = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
