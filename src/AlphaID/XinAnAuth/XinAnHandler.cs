using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace XinAnAuth;

/// <summary>
/// 
/// </summary>
public class XinAnHandler : OAuthHandler<XinAnOptions>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    public XinAnHandler(IOptionsMonitor<XinAnOptions> options,
                        ILoggerFactory logger,
                        UrlEncoder encoder,
                        ISystemClock clock)
        : base(options,
               logger,
               encoder,
               clock)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="properties"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
    {
        if (tokens.AccessToken == null)
            throw new InvalidOperationException("Access token not exists.");
        var endpoint = QueryHelpers.AddQueryString(this.Options.UserInformationEndpoint, "access_token", tokens.AccessToken);

        var response = await this.Options.Backchannel.GetAsync(endpoint, this.Context.RequestAborted);
        response.EnsureSuccessStatusCode();

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(this.Context.RequestAborted));

        var detailInfoEndpoint = QueryHelpers.AddQueryString(this.Options.UserDetailInformationEndpoint, new Dictionary<string, string?>()
        {
            {"access_token", tokens.AccessToken },
            {"oauth_consumer_key", this.Options.ClientId },
            {"openid", payload.RootElement.GetString("openid") },
        });
        
        var detailInfoResponse = await this.Options.Backchannel.GetAsync(detailInfoEndpoint, this.Context.RequestAborted);
        if (!detailInfoResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"获取信安世纪SSO用户详细信息时发生错误，HTTP响应码：{response.StatusCode}。请检查身份验证信息是否正确。");
        }
        var responseContent = await detailInfoResponse.Content.ReadAsStringAsync(this.Context.RequestAborted);
        using var detailPayload = JsonDocument.Parse(await detailInfoResponse.Content.ReadAsStringAsync(this.Context.RequestAborted));
        using var userDoc = JsonDocument.Parse(detailPayload.RootElement.GetString("userinfo")!);
        var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, userDoc.RootElement);
        context.RunClaimActions();
        await this.Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, this.Scheme.Name);
    }
}
