using Newtonsoft.Json;
using System.Text;

namespace IdSubjects.WechatWebLogin;

/// <summary>
/// OAuth2Service
/// </summary>
public class OAuth2Service
{
    private readonly JsonSerializer _jsonSerializer;

    /// <summary>
    /// Initialize OAuth2Service.
    /// </summary>
    public OAuth2Service()
    {
        _jsonSerializer = new JsonSerializer();
    }

    /// <summary>
    /// Get Token via Resource Owner Password Credential Flow.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="userSpn"></param>
    /// <param name="userPassword"></param>
    /// <param name="resource"></param>
    /// <returns></returns>
    public async Task<OAuth2Result?> GetResourceOwnerPasswordCredentialTokenAsync(string clientId, string clientSecret, string userSpn, string userPassword, string resource)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://federal.changingsoft.com")
        };

        var formData = new Dictionary<string, string>
        {
            {"client_id", clientId },
            {"client_secret", clientSecret },
            {"username", userSpn },
            {"password", userPassword },
            {"grant_type", "password" },
            {"resource", resource }
        };

        var response = await httpClient.PostAsync("adfs/oauth2/token", new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"远程返回错误{response.StatusCode}");


        var stream = await response.Content.ReadAsStreamAsync();

        return _jsonSerializer.Deserialize<OAuth2Result>(new JsonTextReader(new StreamReader(stream, Encoding.UTF8)));
    }
}
