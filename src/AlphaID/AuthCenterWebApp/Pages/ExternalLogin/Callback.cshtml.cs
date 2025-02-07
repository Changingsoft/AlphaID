using System.Security.Claims;
using AlphaIdPlatform.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback(
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<Callback> logger,
    UserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    IOptions<LoginOptions> loginOptions) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        //从外部登录认证
        AuthenticateResult result =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true) throw new Exception("外部登录错误。");

        ClaimsPrincipal? externalUser = result.Principal;

        if (logger.IsEnabled(LogLevel.Debug))
        {
            IEnumerable<string> externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the "sub" claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                            externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                            throw new Exception("Unknown userid");

        string provider = result.Properties.Items[".AuthScheme"]!;
        string providerUserId = userIdClaim.Value;
        string returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        NaturalPerson? user = await userManager.FindByLoginAsync(provider, providerUserId);
        // 如果未找到用户，则尝试让用户本地登录以便绑定到外部登录。
        // 如果未启用本地登录，则登录活动失败。
        if (user == null)
        {
            if (loginOptions.Value.AllowLocalLogin)
                return RedirectToPage("/Account/Login", new { returnUrl });
            else
                return RedirectToPage("/Account/LoginFailed");
        }
        
        // 如果用户被禁用，则登录活动失败。
        if(!user.Enabled)
            return RedirectToPage("/Account/LoginFailed");

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for sign out from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // 签发本地登录凭据。 
        await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        //注销当前的外部登录凭据。
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // 处理返回URL。
        // check if external login is in the context of an OIDC request
        AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(returnUrl);
        await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true,
            context?.Client.ClientId));

        if (context != null)
            if (context.IsNativeClient())
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);

        return Redirect(returnUrl);
    }

    /// <summary>
    /// 捕获外部登录上下文。
    /// </summary>
    /// <remarks>
    /// 如果外部登录是基于OIDC，我们需要处理一些事情以便登出（注销）能正常工作。
    /// this will be different for WS-Fed, SAML2p or other protocols.
    /// </remarks>
    /// <param name="externalResult"></param>
    /// <param name="localClaims"></param>
    /// <param name="localSignInProps"></param>
    private void CaptureExternalLoginContext(AuthenticateResult externalResult,
        List<Claim> localClaims,
        AuthenticationProperties localSignInProps)
    {
        // 捕获用于登录的标识提供方（IdP），以便会话知道用户从哪里来的。
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // 如果外部系统发送了会话ID声明，将其复制过来，以便后续可用它做单点登出。
        Claim? sidClaim = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sidClaim != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sidClaim.Value));

        // 如果外部提供器签发了 id_token，我们将其保存以便后续注销。
        string? idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
    }
}