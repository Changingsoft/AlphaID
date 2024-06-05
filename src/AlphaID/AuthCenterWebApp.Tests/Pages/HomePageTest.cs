using AlphaIdPlatform;
using AngleSharp.Html.Dom;
using IntegrationTestUtilities.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthCenterWebApp.Tests.Pages;

[Collection(nameof(TestServerCollection))]
public class HomePageTest(AuthCenterWebAppFactory factory)
{
    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    public async Task AnonymousAccessHomePage(string url)
    {
        HttpClient client = factory.CreateClient();
        var productInfo = factory.Services.GetRequiredService<IOptions<ProductInfo>>();

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        IHtmlDocument doc = await HtmlHelpers.GetDocumentAsync(response);
        Assert.Equal(productInfo.Value.Name, doc.Title);
    }
}