using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AlphaIdPlatform.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using IdSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class BindLoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IEventService events,
    ApplicationUserManager userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<LoginOptions> loginOptions) : PageModel
{
    public ViewModel View { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string returnUrl)
    {
        AuthenticateResult externalLoginResult =
            await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (!externalLoginResult.Succeeded) throw new Exception("无效的外部登录。");

        await BuildModelAsync(returnUrl);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //检查我们是否在授权请求的上下文中
        AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        //用户点击了“取消”按钮
        if (Input.Button != "login")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back access denied OIDC error response to the client.
                await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);

                return Redirect(Input.ReturnUrl);
            }

            //由于我们没有有效的上下文，那么我们只需返回主页
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            AuthenticateResult externalLoginResult =
                await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (!externalLoginResult.Succeeded)
                throw new Exception("无效的外部登录");

            //登录过程。
            ApplicationUser? user = await userManager.FindByEmailAsync(Input.Username)
                                  ?? await userManager.FindByMobileAsync(Input.Username, HttpContext.RequestAborted)
                                  ?? await userManager.FindByNameAsync(Input.Username);
            if (user != null)
            {
                bool passwordOk = await userManager.CheckPasswordAsync(user, Input.Password);
                if (!passwordOk)
                {
                    await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials",
                        clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, loginOptions.Value.InvalidCredentialsErrorMessage);
                    return Page();
                }

                //为用户绑定外部登录
                ClaimsPrincipal? externalUser = externalLoginResult.Principal;
                Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                    externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                    throw new Exception("Unknown userid");

                string? provider = externalLoginResult.Properties.Items[".AuthScheme"];
                string? providerDisplayName = externalLoginResult.Properties.Items["schemeDisplayName"];
                string providerUserId = userIdClaim.Value;
                await userManager.AddLoginAsync(user,
                    new UserLoginInfo(provider!, providerUserId, providerDisplayName));

                // this allows us to collect any additional claims or properties
                // for the specific protocols used and store them in the local auth cookie.
                // this is typically used to store data needed for sign out from those protocols.
                var additionalLocalClaims = new List<Claim>();
                var localSignInProps = new AuthenticationProperties();
                CaptureExternalLoginContext(externalLoginResult, additionalLocalClaims, localSignInProps);


                await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                    clientId: context?.Client.ClientId));

                if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.UtcNow.AddDays(-365.0))
                {
                    ClaimsPrincipal principal = GenerateMustChangePasswordPrincipal(user);
                    await signInManager.SignOutAsync();
                    await HttpContext.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme, principal);
                    return RedirectToPage("ChangePassword", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (context != null)
                {
                    if (context.IsNativeClient())
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(Input.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(Input.ReturnUrl))
                    return Redirect(Input.ReturnUrl);
                if (string.IsNullOrEmpty(Input.ReturnUrl))
                    return Redirect("~/");
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials",
                clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, loginOptions.Value.InvalidCredentialsErrorMessage);
        }

        // something went wrong, show form with error
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(returnUrl);

        //在授权上下文中指定了IdP，因此跳过本地登录而直接转到指定的IdP。
        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            bool local = context.IdP == IdentityServerConstants.LocalIdentityProvider;
            AuthenticationScheme? scheme = await schemeProvider.GetSchemeAsync(context.IdP);
            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel
            {
                EnableLocalLogin = local
            };

            Input.Username = context.LoginHint ?? "";

            if (!local)
                View.ExternalProviders =
                [
                    new ViewModel.ExternalProvider
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme!.DisplayName
                    }
                ];

            return;
        }

        IEnumerable<AuthenticationScheme> schemes = await schemeProvider.GetAllSchemesAsync();

        List<ViewModel.ExternalProvider> providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        IEnumerable<ViewModel.ExternalProvider> dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        Client? client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions.Count != 0)
                providers = providers.Where(provider =>
                    client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
        }

        View = new ViewModel
        {
            AllowRememberLogin = loginOptions.Value.AllowRememberLogin,
            EnableLocalLogin = allowLocal && loginOptions.Value.AllowLocalLogin,
            ExternalProviders = [.. providers]
        };
    }

    private void CaptureExternalLoginContext(AuthenticateResult externalResult,
        List<Claim> localClaims,
        AuthenticationProperties localSignInProps)
    {
        // capture the idp used to log in, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // if the external system sent a session id claim, copy it over.
        // so we can use it for single sign-out
        Claim? sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));

        // if the external provider issued an id_token, we'll keep it for sign out
        string? idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
    }


    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(ApplicationUser person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }

    public class InputModel
    {
        [Display(Name = "User name", Prompt = "Account name, email, mobile phone number, ID card number, etc.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Username { get; set; } = null!;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember my login")]
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; } = null!;

        public string Button { get; set; } = null!;
    }

    public class ViewModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = [];

        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders.Count() == 1;

        public string? ExternalLoginScheme =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? ExternalLoginDisplayName =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.DisplayName : null;

        public class ExternalProvider
        {
            public string? DisplayName { get; set; }
            public string AuthenticationScheme { get; set; } = null!;
        }
    }
}