using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AuthCenterWebApp.Services.WechatMp;

public class WechatMpOAuthHandler : OAuthHandler<WechatMpAuthOptions>
{
    public WechatMpOAuthHandler(IOptionsMonitor<WechatMpAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        // 1. 生成短ID
        var stateId = Guid.NewGuid().ToString("N");

        // 2. 存储属性到服务端（此处用静态字典演示，生产建议用分布式缓存）
        StateStore.Save(stateId, properties);

        // 构造微信授权参数
        var parameters = new Dictionary<string, string?>
        {
            { "appid", Options.ClientId }, // 微信要求用 appid
            { "redirect_uri", redirectUri },
            { "response_type", "code" },
            { "scope", string.Join(' ', Options.Scope) },
            { "state", Options.StateDataFormat.Protect(properties) }
        };

        // 构造最终授权 URL
        var authorizationEndpoint = Options.AuthorizationEndpoint;
        var url = $"{QueryHelpers.AddQueryString(authorizationEndpoint, parameters)}#wechat_redirect";

        return url;
    }

    protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
    {

        // 微信要求的参数
        var parameters = new Dictionary<string, string?>
        {
            { "appid", Options.ClientId },
            { "secret", Options.ClientSecret },
            { "code", context.Code },
            { "grant_type", "authorization_code" },
        };

        var url = QueryHelpers.AddQueryString(Options.TokenEndpoint, parameters);

        var response = await Backchannel.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return OAuthTokenResponse.Failed(new Exception($"微信Token获取失败: {error}"));
        }

        return OAuthTokenResponse.Success(JsonDocument.Parse(await response.Content.ReadAsStringAsync()));
    }

    public class AccessTokenPayload
    {
        public string? AccessToken { get; set; }
        public string? OpenId { get; set; }
        public string? Scope { get; set; }
        public int? ExpiresIn { get; set; }

        public int? Errcode { get; set; }
        public string? Errmsg { get; set; }
    }

    protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
    {
        var baseTicket = await base.CreateTicketAsync(identity, properties, tokens);
        // 1. 获取 openid
        var openId = tokens.Response!.RootElement.TryGetProperty("openid", out var openIdElement)
            ? openIdElement.GetString()
            : null;

        if (!string.IsNullOrEmpty(openId))
        {
            // 2. 添加 openid 到 ClaimsIdentity
            identity.AddClaim(new Claim("openid", openId));
            // 你也可以用 ClaimTypes.NameIdentifier 或自定义类型
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, openId));
        }

        // 3. 继续后续流程（如有需要可添加其它 claims）
        return new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Scheme.Name);
    }

    // 简单的服务端存储（生产建议用分布式缓存）
    public static class StateStore
    {
        private static readonly Dictionary<string, AuthenticationProperties> s_store = new();

        public static void Save(string key, AuthenticationProperties props)
        {
            s_store[key] = props;
        }

        public static AuthenticationProperties? Get(string key)
        {
            s_store.TryGetValue(key, out var props);
            return props;
        }

        public static void Remove(string key)
        {
            s_store.Remove(key);
        }
    }
}
