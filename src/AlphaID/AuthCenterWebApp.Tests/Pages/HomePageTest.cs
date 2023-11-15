using AlphaIdPlatform;
using IntegrationTestUtilities.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthCenterWebApp.Tests.Pages;
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
    public async Task AnonymousAccessHomePage(string url)
    {
        var client = this.factory.CreateClient();
        var productInfo = this.factory.Services.GetRequiredService<IOptions<ProductInfo>>();

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var doc = await HtmlHelpers.GetDocumentAsync(response);
        Assert.Equal(productInfo.Value.Name, doc.Title);
    }
}
