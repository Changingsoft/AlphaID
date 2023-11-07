using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using IDSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class BindLoginModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly SignInManager<NaturalPerson> signInManager;
    private readonly IIdentityServerInteractionService interaction;
    private readonly IEventService events;
    private readonly IAuthenticationSchemeProvider schemeProvider;
    private readonly IIdentityProviderStore identityProviderStore;



    public BindLoginModel(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events,
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.interaction = interaction;
        this.schemeProvider = schemeProvider;
        this.identityProviderStore = identityProviderStore;
        this.events = events;
    }

    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGet(string returnUrl)
    {
        var externalLoginResult = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!externalLoginResult.Succeeded)
        {
            throw new Exception("无效的外部登录。");
        }

        await this.BuildModelAsync(returnUrl);

        return this.Page();
    }

    public async Task<IActionResult> OnPost()
    {
        //检查我们是否在授权请求的上下文中
        var context = await this.interaction.GetAuthorizationContextAsync(this.Input.ReturnUrl);

        //用户点击了“取消”按钮
        if (this.Input.Button != "login")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await this.interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(this.Input.ReturnUrl);
                }

                return this.Redirect(this.Input.ReturnUrl);
            }
            else
            {
                //由于我们没有有效的上下文，那么我们只需返回主页
                return this.Redirect("~/");
            }
        }

        if (this.ModelState.IsValid)
        {
            var externalLoginResult = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!externalLoginResult.Succeeded)
                throw new Exception("无效的外部登录");

            //登录过程。
            var user = await this.userManager.FindByEmailAsync(this.Input.Username)
                ?? await this.userManager.FindByMobileAsync(this.Input.Username)
                ?? await this.userManager.FindByNameAsync(this.Input.Username);
            if (user != null)
            {
                var passwordOk = await this.userManager.CheckPasswordAsync(user, this.Input.Password);
                if (!passwordOk)
                {
                    await this.events.RaiseAsync(new UserLoginFailureEvent(this.Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
                    this.ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
                    return this.Page();
                }

                //为用户绑定外部登录
                var externalUser = externalLoginResult.Principal;
                var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

                var provider = externalLoginResult.Properties.Items[".AuthScheme"];
                var providerDisplayName = externalLoginResult.Properties.Items["schemeDisplayName"];
                var providerUserId = userIdClaim.Value;
                var returnUrl = externalLoginResult.Properties.Items["returnUrl"];
                await this.userManager.AddLoginAsync(user, new UserLoginInfo(provider!, providerUserId, providerDisplayName));

                // this allows us to collect any additional claims or properties
                // for the specific protocols used and store them in the local auth cookie.
                // this is typically used to store data needed for signout from those protocols.
                var additionalLocalClaims = new List<Claim>();
                var localSignInProps = new AuthenticationProperties();
                this.CaptureExternalLoginContext(externalLoginResult, additionalLocalClaims, localSignInProps);


                await this.signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

                // delete temporary cookie used during external authentication
                await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                await this.events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.UtcNow.AddDays(-365.0))
                {
                    var principal = this.GenerateMustChangePasswordPrincipal(user);
                    await this.signInManager.SignOutAsync();
                    await this.HttpContext.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme, principal);
                    return this.RedirectToPage("ChangePassword", new { this.Input.ReturnUrl, RememberMe = this.Input.RememberLogin });
                }

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(this.Input.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return this.Redirect(this.Input.ReturnUrl);
                }

                // request for a local page
                if (this.Url.IsLocalUrl(this.Input.ReturnUrl))
                {
                    return this.Redirect(this.Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(this.Input.ReturnUrl))
                {
                    return this.Redirect("~/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }

            await this.events.RaiseAsync(new UserLoginFailureEvent(this.Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
            this.ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
        }

        // something went wrong, show form with error
        await this.BuildModelAsync(this.Input.ReturnUrl);
        return this.Page();
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        this.Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        var context = await this.interaction.GetAuthorizationContextAsync(returnUrl);

        //在授权上下文中指定了IdP，因此跳过本地登录而直接转到指定的IdP。
        if (context?.IdP != null && await this.schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;
            var scheme = await this.schemeProvider.GetSchemeAsync(context.IdP);
            // this is meant to short circuit the UI and only trigger the one external IdP
            this.View = new ViewModel
            {
                EnableLocalLogin = local,
            };

            this.Input.Username = context.LoginHint ?? "";

            if (!local)
            {
                this.View.ExternalProviders = new ViewModel.ExternalProvider[]
                {
                    new ViewModel.ExternalProvider()
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme!.DisplayName,
                    }
                };
            }

            return;
        }

        var schemes = await this.schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dyanmicSchemes = (await this.identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dyanmicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
            {
                providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        this.View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }

    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to login, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties!.Items[".AuthScheme"]!));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }


    private ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
    {
        var identity = new ClaimsIdentity(IdSubjectsIdentityDefaults.MustChangePasswordScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, person.Id));
        return new ClaimsPrincipal(identity);
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "User name", Prompt = "Account name, email, mobile phone number, ID card number, etc.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Password")]
        public string Password { get; init; } = default!;

        [Display(Name = "Remember my login")]
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; init; } = default!;

        public string Button { get; set; } = default!;
    }

    public class ViewModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; init; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => this.ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders.Count() == 1;
        public string? ExternalLoginScheme => this.IsExternalLoginOnly ? this.ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? EnternalLoginDisplayName => this.IsExternalLoginOnly ? this.ExternalProviders.SingleOrDefault()?.DisplayName : null;

        public class ExternalProvider
        {
            public string? DisplayName { get; init; }
            public string AuthenticationScheme { get; init; } = default!;
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
