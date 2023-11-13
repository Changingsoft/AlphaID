using System.Net;
using Xunit;

namespace AlphaIDWebAPITests.Controllers;

[Collection(nameof(TestServerCollection))]
public class OidcControllerTests
{
    private readonly AlphaIdApiFactory factory;

    public OidcControllerTests(AlphaIdApiFactory factory)
    {
        this.factory = factory;
    }


    [Fact]
    public async Task GetClientNameByClientId()
    {
        var client = this.factory.CreateAuthenticatedClient();

        var response = await client.GetAsync($"api/Oidc/Client/{WebUtility.UrlEncode("d70700eb-c4d8-4742-a79a-6ecf2064b27c")}");
        Assert.True(response.IsSuccessStatusCode);
    }
}
