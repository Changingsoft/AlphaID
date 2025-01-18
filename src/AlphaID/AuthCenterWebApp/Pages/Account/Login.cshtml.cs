using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using Duende.IdentityServer;
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
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class LoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IEventService events,
    ApplicationUserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    IOptions<LoginOptions> loginOptions) : PageModel
{
    public ViewModel View { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string? returnUrl)
    {
        await BuildModelAsync(returnUrl);
        if (View.IsExternalLoginOnly)
            // we only have one option for logging in and it's an external provider
            return RedirectToPage("/ExternalLogin/Challenge",
                new
                {
                    scheme = View.ExternalLoginScheme, schemeDisplayName = View.ExternalLoginDisplayName, returnUrl
                });

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
                // 将用户的取消发送回 IdentityServer，以便它能拒绝 consent（即便客户端不需要consent）。
                // 这将会把访问被拒绝的响应发送回客户端。
                await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl!);

                return Redirect(Input.ReturnUrl!);
            }

            //由于我们没有有效的上下文，那么我们只需返回主页
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            //登录过程。
            NaturalPerson? user = await userManager.FindByEmailAsync(Input.Username)
                                  ?? await userManager.FindByMobileAsync(Input.Username, HttpContext.RequestAborted)
                                  ?? await userManager.FindByNameAsync(Input.Username);
            if (user != null)
            {
                SignInResult result =
                    await signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberLogin, true);
                if (result.Succeeded)
                {
                    await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                        clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl!);

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl!);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl)) return Redirect(Input.ReturnUrl);

                    if (string.IsNullOrEmpty(Input.ReturnUrl)) return Redirect("~/");

                    // user might have clicked on a malicious link - should be logged
                    throw new ArgumentException("invalid return URL");
                }

                if (result.MustChangePassword())
                    return RedirectToPage("ChangePassword", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });

                if (result.RequiresTwoFactor)
                    return RedirectToPage("./LoginWith2fa", new { Input.ReturnUrl, RememberMe = Input.RememberLogin });

                if (result.IsLockedOut)
                    //_logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
            }


            await events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials",
                clientId: context?.Client.ClientId));
            ModelState.AddModelError(string.Empty, loginOptions.Value.InvalidCredentialsErrorMessage);
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
                    new ExternalProvider
                    {
                        AuthenticationScheme = context.IdP,
                        DisplayName = scheme!.DisplayName!
                    }
                ];

            return;
        }

        // 载入所有身份验证方案。
        IEnumerable<AuthenticationScheme> schemes = await schemeProvider.GetAllSchemesAsync();

        List<ExternalProvider> providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        //从存储加载验证方案。
        IEnumerable<ExternalProvider> dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName ?? ""
            });
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        Client? client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin; //客户端是否允许本地登录
            if (client.IdentityProviderRestrictions.Count != 0)
                //过滤只有客户端允许的登录提供器。
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


    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "User name", Prompt = "Account name, email, mobile phone number, ID card number, etc.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me on this device")]
        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; }

        public string Button { get; set; } = null!;
    }

    public class ViewModel
    {
        /// <summary>
        ///     获取或设置是否允许记住登录。
        /// </summary>
        public bool AllowRememberLogin { get; set; } = true;

        /// <summary>
        ///     获取或设置是否启用本地登录。
        /// </summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        ///     获取所有外部登录提供器。
        /// </summary>
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = [];

        /// <summary>
        ///     获取可见的外部登录提供器。
        /// </summary>
        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary>
        ///     只是是否仅外部登录。当且仅当禁用本地登录，且外部登录提供器只有1个时，该属性为true。
        /// </summary>
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders.Count() == 1;

        public string? ExternalLoginScheme =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? ExternalLoginDisplayName =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.DisplayName : null;
    }

    public class ExternalProvider
    {
        public string DisplayName { get; set; } = null!;
        public string AuthenticationScheme { get; set; } = null!;
    }
}