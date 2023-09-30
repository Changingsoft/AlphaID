using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace AlphaIDWebAPITests.Controllers;

[Collection(nameof(TestServerCollection))]
public class OidcControllerTests
{
    private readonly AlphaIDAPIFactory factory;
    private readonly TokenManager tokenManager;

    public OidcControllerTests(AlphaIDAPIFactory factory, TokenManager tokenManager)
    {
        this.factory = factory;
        this.tokenManager = tokenManager;
    }


    [Fact]
    public async Task GetClientNameByClientId()
    {
        var client = this.factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this.tokenManager.GetAccessTokenAsync());

        var response = await client.GetAsync($"api/Oidc/Client/{WebUtility.UrlEncode("d70700eb-c4d8-4742-a79a-6ecf2064b27c")}");
        Assert.True(response.IsSuccessStatusCode);
    }
}
