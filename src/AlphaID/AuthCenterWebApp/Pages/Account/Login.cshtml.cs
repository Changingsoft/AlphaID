using AuthCenterWebApp.Services;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IDSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly SignInManager<NaturalPerson> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;

    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public LoginModel(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events,
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._interaction = interaction;
        this._schemeProvider = schemeProvider;
        this._identityProviderStore = identityProviderStore;
        this._events = events;
    }

    public async Task<IActionResult> OnGet(string returnUrl)
    {
        await this.BuildModelAsync(returnUrl);

        if (this.View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            return this.RedirectToPage("/ExternalLogin/Challenge", new { scheme = this.View.ExternalLoginScheme, schemeDisplayName = this.View.EnternalLoginDisplayName, returnUrl });
        }

        return this.Page();
    }

    public async Task<IActionResult> OnPost()
    {
        //检查我们是否在授权请求的上下文中
        var context = await this._interaction.GetAuthorizationContextAsync(this.Input.ReturnUrl);

        //用户点击了“取消”按钮
        if (this.Input.Button != "login")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await this._interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

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
            //登录过程。
            var user = await this._userManager.FindByEmailAsync(this.Input.Username)
                ?? await this._userManager.FindByMobileAsync(this.Input.Username)
                ?? await this._userManager.FindByNameAsync(this.Input.Username);
            if (user != null)
            {
                var result = await this._signInManager.PasswordSignInAsync(user, this.Input.Password, this.Input.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await this._events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

                    if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.Now.AddDays(-365.0))
                    {
                        var principal = this.GenerateMustChangePasswordPrincipal(user);
                        await this._signInManager.SignOutAsync();
                        await this.HttpContext.SignInAsync(AuthCenterIdentitySchemes.MustChangePasswordScheme, principal);
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

                if (result.RequiresTwoFactor)
                {
                    if (!user.PasswordLastSet.HasValue || user.PasswordLastSet.Value < DateTime.Now.AddDays(-365.0))
                    {
                        var principal = this.GenerateMustChangePasswordPrincipal(user);
                        await this._signInManager.SignOutAsync();
                        await this.HttpContext.SignInAsync(AuthCenterIdentitySchemes.MustChangePasswordScheme, principal);
                        return this.RedirectToPage("ChangePassword", new { this.Input.ReturnUrl, RememberMe = this.Input.RememberLogin });
                    }
                    return this.RedirectToPage("./LoginWith2fa", new { this.Input.ReturnUrl, RememberMe = this.Input.RememberLogin });
                }

                if (result.IsLockedOut)
                {
                    //_logger.LogWarning("User account locked out.");
                    return this.RedirectToPage("./Lockout");
                }
            }


            await this._events.RaiseAsync(new UserLoginFailureEvent(this.Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
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

        var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);

        //在授权上下文中指定了IdP，因此跳过本地登录而直接转到指定的IdP。
        if (context?.IdP != null && await this._schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;
            var scheme = await this._schemeProvider.GetSchemeAsync(context.IdP);
            // this is meant to short circuit the UI and only trigger the one external IdP
            this.View = new ViewModel
            {
                EnableLocalLogin = local,
            };

            this.Input.Username = context?.LoginHint;

            if (!local)
            {
                this.View.ExternalProviders = new ViewModel.ExternalProvider[]
                {
                    new ViewModel.ExternalProvider()
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme.DisplayName,
                    }
                };
            }

            return;
        }

        var schemes = await this._schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dyanmicSchemes = (await this._identityProviderStore.GetAllSchemeNamesAsync())
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

    private ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
    {
        var identity = new ClaimsIdentity(AuthCenterIdentitySchemes.MustChangePasswordScheme);
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
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me on this device")]
        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }

        public string Button { get; set; } = default!;
    }

    public class ViewModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => this.ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => this.EnableLocalLogin == false && this.ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        public string EnternalLoginDisplayName => this.IsExternalLoginOnly ? this.ExternalProviders?.SingleOrDefault()?.DisplayName : null;

        public class ExternalProvider
        {
            public string DisplayName { get; set; }
            public string AuthenticationScheme { get; set; }
        }
    }

    public class LoginOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static string InvalidCredentialsErrorMessage = "用户名或密码无效";
    }
}