using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdSubjects;
using IdSubjects.ChineseName;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback(
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<Callback> logger,
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ChinesePersonNamePinyinConverter chinesePersonNamePinyinConverter) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        //从外部登录认证
        var result = await this.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true)
        {
            throw new Exception("外部登录错误。");
        }

        var externalUser = result.Principal;

        if (logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the "sub" claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

        var provider = result.Properties.Items[".AuthScheme"]!;
        var providerUserId = userIdClaim.Value;
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        var user = await userManager.FindByLoginAsync(provider, providerUserId);
        if (user == null)
        {
            return this.RedirectToPage("/Account/BindLogin", new { returnUrl });
        }

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for sign out from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        this.CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        //注销外部登录
        await this.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        // check if external login is in the context of an OIDC request
        var context = await interaction.GetAuthorizationContextAsync(returnUrl);
        await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }

        return this.Redirect(returnUrl);
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to log in, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sidClaim = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sidClaim != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sidClaim.Value));
        }

        // if the external provider issued an id_token, we'll keep it for sign out
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
        }
    }
}