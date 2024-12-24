using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdSubjects;
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
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        //���ⲿ��¼��֤
        AuthenticateResult result =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true) throw new Exception("�ⲿ��¼����");

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
        if (user == null) return RedirectToPage("/Account/BindLogin", new { returnUrl });

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for sign out from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // ǩ�����ص�¼ƾ�ݡ�
        await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        //ע����ǰ���ⲿ��¼ƾ�ݡ�
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // ������URL��
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
    /// �����ⲿ��¼�����ġ�
    /// </summary>
    /// <remarks>
    /// ����ⲿ��¼�ǻ���OIDC��������Ҫ����һЩ�����Ա�ǳ���ע����������������
    /// this will be different for WS-Fed, SAML2p or other protocols.
    /// </remarks>
    /// <param name="externalResult"></param>
    /// <param name="localClaims"></param>
    /// <param name="localSignInProps"></param>
    private void CaptureExternalLoginContext(AuthenticateResult externalResult,
        List<Claim> localClaims,
        AuthenticationProperties localSignInProps)
    {
        // �������ڵ�¼�ı�ʶ�ṩ����IdP�����Ա�Ự֪���û����������ġ�
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // ����ⲿϵͳ�����˻ỰID���������临�ƹ������Ա����������������ǳ���
        Claim? sidClaim = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sidClaim != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sidClaim.Value));

        // ����ⲿ�ṩ��ǩ���� id_token�����ǽ��䱣���Ա����ע����
        string? idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
    }
}