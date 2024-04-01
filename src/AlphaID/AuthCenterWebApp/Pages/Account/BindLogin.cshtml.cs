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
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class BindLoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IEventService events,
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager) : PageModel
{
    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string returnUrl)
    {
        var externalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (!externalLoginResult.Succeeded)
        {
            throw new Exception("无效的外部登录。");
        }

        await BuildModelAsync(returnUrl);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //检查我们是否在授权请求的上下文中
        var context = await interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

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
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl);
            }
            else
            {
                //由于我们没有有效的上下文，那么我们只需返回主页
                return Redirect("~/");
            }
        }

        if (ModelState.IsValid)
        {
            var externalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (!externalLoginResult.Succeeded)
                throw new Exception("无效的外部登录");

            //登录过程。
            var user = await userManager.FindByEmailAsync(Input.Username)
                ?? await userManager.FindByMobileAsync(Input.Username, HttpContext.RequestAborted)
                ?? await userManager.FindByNameAsync(Input.Username);
            if (user != null)
            {
                var passwordOk = await userManager.CheckPasswordAsync(user, Input.Password);
                if (!passwordOk)
                {
                    await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
                    return Page();
                }

                //为用户绑定外部登录
                var externalUser = externalLoginResult.Principal;
                var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

                var provider = externalLoginResult.Properties.Items[".AuthScheme"];
                var providerDisplayName = externalLoginResult.Properties.Items["schemeDisplayName"];
                var providerUserId = userIdClaim.Value;
                await userManager.AddLoginAsync(user, new UserLoginInfo(provider!, providerUserId, providerDisplayName));

                // this allows us to collect any additional claims or properties
                // for the specific protocols used and store them in the local auth cookie.
                // this is typically used to store data needed for sign out from those protocols.
                var additionalLocalClaims = new List<Claim>();
                var localSignInProps = new AuthenticationProperties();
                CaptureExternalLoginContext(externalLoginResult, additionalLocalClaims, localSignInProps);


                await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

                // delete temporary cookie used during external authentication
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

                await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.UtcNow.AddDays(-365.0))
                {

/* 项目“AuthCenterWebApp (net6.0)”的未合并的更改
在此之前:
                    var principal = this.GenerateMustChangePasswordPrincipal(user);
在此之后:
                    var principal = GenerateMustChangePasswordPrincipal(user);
*/
                    var principal = GenerateMustChangePasswordPrincipal(user);
                    await signInManager.SignOutAsync();
                    await HttpContext.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme, principal);
                    return RedirectToPage("ChangePassword", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(Input.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(Input.ReturnUrl))
                {
                    return Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }

            await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
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

        var context = await interaction.GetAuthorizationContextAsync(returnUrl);

        //在授权上下文中指定了IdP，因此跳过本地登录而直接转到指定的IdP。
        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;
            var scheme = await schemeProvider.GetSchemeAsync(context.IdP);
            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel
            {
                EnableLocalLogin = local,
            };

            Input.Username = context.LoginHint ?? "";

            if (!local)
            {
                View.ExternalProviders =
                [
                    new()
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme!.DisplayName,
                    }
                ];
            }

            return;
        }

        var schemes = await schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions.Count != 0)
            {
                providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = [.. providers]
        };
    }

    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to log in, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // if the external system sent a session id claim, copy it over so we can use it for single sign-out
        var sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for sign out
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = idToken }]);
        }
    }


    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }

    public class InputModel
    {
        [Display(Name = "User name", Prompt = "Account name, email, mobile phone number, ID card number, etc.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Username { get; set; } = default!;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember my login")]
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; } = default!;

        public string Button { get; set; } = default!;
    }

    public class ViewModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = [];
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders.Count() == 1;
        public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? ExternalLoginDisplayName => IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.DisplayName : null;

        public class ExternalProvider
        {
            public string? DisplayName { get; set; }
            public string AuthenticationScheme { get; set; } = default!;
        }
    }

    public class LoginOptions
    {
        public static readonly bool AllowLocalLogin = true;
        public static readonly bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static readonly string InvalidCredentialsErrorMessage = "用户名或密码无效";
    }

}
