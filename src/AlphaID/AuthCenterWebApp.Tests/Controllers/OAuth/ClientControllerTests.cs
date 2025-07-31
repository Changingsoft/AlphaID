using System.Net;
using System.Net.Http.Json;

namespace AuthCenterWebApp.Tests.Controllers.OAuth;

[Collection(nameof(TestServerCollection))]
public class ClientControllerTests(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GetClientName()
    {
        HttpClient client = factory.CreateBearerTokenClient();

        HttpResponseMessage response = await client.GetAsync("api/OAuth/Client/d70700eb-c4d8-4742-a79a-6ecf2064b27c");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ClientModel>();
        Assert.Equal("AlphaID Management Center", result!.Name);
    }

    [Fact]
    public async Task GetNonExistsClientName()
    {
        HttpClient client = factory.CreateBearerTokenClient();

        HttpResponseMessage response = await client.GetAsync("api/OAuth/Client/non-exists-client-id");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public record ClientModel(string Name);
}