using AlphaIdPlatform;
using IntegrationTestUtilities.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthCenterWebApp.Tests.Pages;
public class HomePageTest(AuthCenterWebAppFactory factory) : IClassFixture<AuthCenterWebAppFactory>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    public async Task AnonymousAccessHomePage(string url)
    {
        var client = factory.CreateClient();
        var productInfo = factory.Services.GetRequiredService<IOptions<ProductInfo>>();

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var doc = await HtmlHelpers.GetDocumentAsync(response);
        Assert.Equal(productInfo.Value.Name, doc.Title);
    }
}
