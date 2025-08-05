using System.Net;

namespace AdminWebApp.Tests;
public class SiteScopeFuncTest
{
    [Fact]
    public async Task RobotsExistsAsync()
    {
        var factory = new AdminWebAppFactory();
        var client = factory.CreateClient();
        var response = await client.GetAsync("/robots.txt");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);
    }
}
