using AlphaIdPlatform.Identity;
using AuthCenterWebApp.Services;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdSubjects;
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
public class LoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IEventService events,
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<LoginModel>? logger) : PageModel
{
    public ViewModel View { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string? returnUrl)
    {
        await BuildModelAsync(returnUrl);
        if (View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            logger?.LogInformation("we only have one option for logging in and it's an external provider");
            return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, schemeDisplayName = View.ExternalLoginDisplayName, returnUrl });
        }

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
                // 将用户的取消发送回 IdentityServer，以便它能拒绝 consent（即便客户端不需要consent）。
                // 这将会把访问被拒绝的响应发送回客户端。
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back access denied OIDC error response to the client.
                await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl!);
                }

                return Redirect(Input.ReturnUrl!);
            }

            //由于我们没有有效的上下文，那么我们只需返回主页
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            //登录过程。
            var user = await userManager.FindByEmailAsync(Input.Username)
                ?? await userManager.FindByMobileAsync(Input.Username, HttpContext.RequestAborted)
                ?? await userManager.FindByNameAsync(Input.Username);
            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl!);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl!);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }

                    if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }

                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }

                if (result.MustChangePassword())
                {
                    var principal = GenerateMustChangePasswordPrincipal(user);
                    await signInManager.SignOutAsync();
                    await HttpContext.SignInAsync(IdSubjectsIdentityDefaults.MustChangePasswordScheme, principal);
                    return RedirectToPage("ChangePassword", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });
                }

                if (result.IsLockedOut)
                {
                    //_logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
            }


            await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials", clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
        }

        // something went wrong, show form with error
        await BuildModelAsync(Input.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string? returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        //获取授权上下文。
        var context = await interaction.GetAuthorizationContextAsync(returnUrl);

        //在授权上下文中指定了IdP，因此跳过本地登录而直接转到指定的IdP。
        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;
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
                        DisplayName = scheme!.DisplayName!,
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

        //从存储加载验证方案。
        var dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName ?? "",
            });
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin; //客户端是否允许本地登录
            if (client.IdentityProviderRestrictions.Count != 0)
            {
                //过滤只有客户端允许的登录提供器。
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

    private static ClaimsPrincipal GenerateMustChangePasswordPrincipal(NaturalPerson person)
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
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me on this device")]
        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; }

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
            public string DisplayName { get; set; } = default!;
            public string AuthenticationScheme { get; set; } = default!;
        }
    }

    /// <summary>
    /// 登录选项。
    /// </summary>
    public class LoginOptions
    {
        public static readonly bool AllowLocalLogin = true;
        public static readonly bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static readonly string InvalidCredentialsErrorMessage = "用户名或密码无效";
    }
}