using AuthCenterWebAppTests;

namespace AlphaIDWebAPITests;
public class TokenManager
{
    private static Token? token;

    public TokenManager()
    {
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var token = await this.EnsureTokenAsync();
        return token.AccessToken;
    }

    private async Task<Token> EnsureTokenAsync()
    {
        if (token == null || token.Expires < DateTime.UtcNow)
        {
            token = await this.GrantTokenFromIdP();
        }
        return token;
    }

    private async Task<Token> GrantTokenFromIdP()
    {
        using var authClient = AuthCenterWebAppFactory.Instance.CreateClient();


        var formDict = new Dictionary<string, string>
        {
            { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
            { "client_secret", "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^" },
            { "scope", "openid profile user_impersonation realname" },
            { "username", "liubei@sanguo.net" },
            { "password", "Pass123$" },
            { "grant_type", "password" },
            //{ "resource", "http://webapi.changingsoft.com/adfs/services/trust" },
        };
        var formContent = new FormUrlEncodedContent(formDict);
        var authResponse = await authClient.PostAsync("/connect/token", formContent);
        authResponse.EnsureSuccessStatusCode();

        var authResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(await authResponse.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException("无法从响应消息中取得令牌结果");
        int expiresin = (int)authResult.expires_in;
        return new Token((string)authResult.access_token, DateTime.UtcNow.AddSeconds(expiresin));
    }
}
