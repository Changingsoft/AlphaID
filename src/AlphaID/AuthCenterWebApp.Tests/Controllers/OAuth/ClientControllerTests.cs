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

        HttpResponseMessage response = await client.GetAsync("api/OAuth/Client/43670b09-b161-46ca-b59a-c0fbde526394", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ClientModel>(TestContext.Current.CancellationToken);
        Assert.Equal("AlphaID AuthCenter Swagger UI", result!.Name);
    }

    [Fact]
    public async Task GetNonExistsClientName()
    {
        HttpClient client = factory.CreateBearerTokenClient();

        HttpResponseMessage response = await client.GetAsync("api/OAuth/Client/non-exists-client-id", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public class ClientModel
    {
        public string Name { get; set; } = null!;

        public DateTime UpdatedAt { get; set; }
    };
}