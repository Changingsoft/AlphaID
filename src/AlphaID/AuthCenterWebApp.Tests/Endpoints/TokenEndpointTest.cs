using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AuthCenterWebApp.Tests.Endpoints;

[Collection(nameof(TestServerCollection))]
public class TokenEndpointTest(AuthCenterWebAppFactory factory)
{
    [Fact]
    public async Task GrantByClientCredentials()
    {
        HttpClient client = factory.CreateClient();
        var forms = new Dictionary<string, string>
        {
            { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
            { "client_secret", "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^" },
            { "grant_type", "client_credentials" }
        };
        HttpResponseMessage response = await client.PostAsync("/connect/token", new FormUrlEncodedContent(forms));
        response.EnsureSuccessStatusCode();
        var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal("Bearer", tokenData!.TokenType);
    }

    [Fact]
    public async Task GrantByPasswordOwner()
    {
        HttpClient client = factory.CreateClient();
        var forms = new Dictionary<string, string>
        {
            { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
            { "client_secret", "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^" },
            { "username", "liubei" },
            { "password", "Pass123$" },
            { "grant_type", "password" }
        };
        HttpResponseMessage response = await client.PostAsync("/connect/token", new FormUrlEncodedContent(forms));
        response.EnsureSuccessStatusCode();
        var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal("Bearer", tokenData!.TokenType);
    }

    [Fact]
    public async Task GetTokenRateLimitTest()
    {
        var client = factory.CreateClient();
        var forms = new Dictionary<string, string>
        {
            { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
            { "client_secret", "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^" },
            { "grant_type", "client_credentials" }
        };
        HttpResponseMessage response = null!;
        for (int i = 0; i < 50; i++)
        {
            response = await client.PostAsync("/connect/token", new FormUrlEncodedContent(forms));
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    public record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = null!;
    }
}