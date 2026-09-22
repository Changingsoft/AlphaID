using System.Net;

namespace AuthCenterWebApp.Tests;

[Collection<TestServerCollection>]
public class SiteScopeFuncTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task RobotsExistsAsync()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/robots.txt", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);
    }
}
