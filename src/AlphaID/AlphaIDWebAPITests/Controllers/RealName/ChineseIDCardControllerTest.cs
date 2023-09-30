using System.Net;
using Xunit;

namespace AlphaIDWebAPITests.Controllers.RealName;

[Collection(nameof(TestServerCollection))]
public class ChineseIDCardControllerTest
{
    private readonly AlphaIDAPIFactory webAPIFactory;
    private readonly TokenManager tokenManager;

    public ChineseIDCardControllerTest(AlphaIDAPIFactory webAPIFactory, TokenManager tokenManager)
    {
        this.webAPIFactory = webAPIFactory;
        this.tokenManager = tokenManager;
    }

    [Fact]
    public async Task GetNonExistsChineseIDCard()
    {
        using var client = this.webAPIFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await this.tokenManager.GetAccessTokenAsync());

        var response = await client.GetAsync("/api/RealName/ChineseIDCardInfo/12345");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetExistsChineseIDCard()
    {
        using var client = this.webAPIFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await this.tokenManager.GetAccessTokenAsync());

        var response = await client.GetAsync("/api/RealName/ChineseIDCard/d2480421-8a15-4292-8e8f-06985a1f645b");
        //404
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
