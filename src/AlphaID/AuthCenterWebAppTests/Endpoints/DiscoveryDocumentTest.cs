namespace AuthCenterWebAppTests.Endpoints;
public class DiscoveryDocumentTest : IClassFixture<AuthCenterWebAppFactory>
{
    private readonly AuthCenterWebAppFactory factory;

    public DiscoveryDocumentTest(AuthCenterWebAppFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task DocumentOk()
    {
        var client = this.factory.CreateClient();
        var response = await client.GetAsync("/.well-known/openid-configuration");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }
}
