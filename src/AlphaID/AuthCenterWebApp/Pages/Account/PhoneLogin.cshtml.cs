using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using BotDetect.Web.Mvc;
using Duende.IdentityModel;
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
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class PhoneLoginModel(
    IIdentityServerInteractionService interaction,
    IAuthenticationSchemeProvider schemeProvider,
    IIdentityProviderStore identityProviderStore,
    IServiceProvider serviceProvider,
    IEventService events,
    ApplicationUserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    IOptions<LoginOptions> loginOptions) : PageModel
{
    public LoginOptionsModel Model { get; set; } = null!;

    public AuthenticateResult ExternalLoginResult { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Phone number")]
    [Required(ErrorMessage = "Validate_Required")]
    [StringLength(14, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
    public string Mobile { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "Verification code")]
    public string VerificationCode { get; set; } = null!;


    [Display(Name = "Captcha code")]
    [Required(ErrorMessage = "Validate_Required")]
    [CaptchaModelStateValidation("LoginCaptcha", ErrorMessage = "Captcha_Invalid")]
    [BindProperty]
    public string CaptchaCode { get; set; } = null!;

    public IVerificationCodeService? VerificationCodeService => serviceProvider.GetService<IVerificationCodeService>();

    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        if (VerificationCodeService is null)
            throw new InvalidOperationException("没有为系统配置短信验证码服务。");

        await BuildModelAsync(returnUrl);

        //尝试验证外部登录。
        ExternalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        if (Model.IsExternalLoginOnly)
            // we only have one option for logging in and it's an external provider
            return RedirectToPage("/ExternalLogin/Challenge",
                new
                {
                    scheme = Model.ExternalLoginScheme,
                    schemeDisplayName = Model.ExternalLoginDisplayName,
                    returnUrl
                });

        return Page();
    }

    public async Task<IActionResult> OnPostSendVerificationCode(string mobile)
    {

        if (!MobilePhoneNumber.TryParse(mobile, out MobilePhoneNumber phoneNumber)) return new JsonResult("移动电话号码无效。");
        await VerificationCodeService!.SendAsync(phoneNumber.ToString());
        return new JsonResult(true);
    }

    public async Task<IActionResult> OnPost()
    {

        //先尝试验证外部登录。
        ExternalLoginResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

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
            NaturalPerson? user = await userManager.FindByMobileAsync(Mobile);
            if (user != null)
            {
                var verificationResult = await VerificationCodeService!.VerifyAsync(Mobile, VerificationCode);
                if (verificationResult)
                {
                    await signInManager.SignInAsync(user, Input.RememberLogin);
                    await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                        clientId: context?.Client.ClientId));

                    //如果外部登录有效，则为用户创建外部登录关联。
                    if (ExternalLoginResult.Succeeded)
                    {
                        //为用户绑定外部登录
                        ClaimsPrincipal? externalUser = ExternalLoginResult.Principal;
                        Claim userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                            externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                            throw new Exception("Unknown userid");

                        string? provider = ExternalLoginResult.Properties.Items[".AuthScheme"];
                        string? providerDisplayName = ExternalLoginResult.Properties.Items["schemeDisplayName"];
                        string providerUserId = userIdClaim.Value;
                        await userManager.AddLoginAsync(user,
                            new UserLoginInfo(provider!, providerUserId, providerDisplayName));

                        // this allows us to collect any additional claims or properties
                        // for the specific protocols used and store them in the local auth cookie.
                        // this is typically used to store data needed for sign out from those protocols.
                        var additionalLocalClaims = new List<Claim>();
                        var localSignInProps = new AuthenticationProperties();
                        CaptureExternalLoginContext(ExternalLoginResult, additionalLocalClaims, localSignInProps);

                        await signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

                        // delete temporary cookie used during external authentication
                        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
                    }

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
            }

            await events.RaiseAsync(new UserLoginFailureEvent(Mobile, "invalid credentials",
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

        //根据returnUrl，获取IdentityServer的授权上下文。
        AuthorizationRequest? context = await interaction.GetAuthorizationContextAsync(returnUrl);

        //如果授权上下文中指定了IdP，则跳过本地登录，向指定的IdP发起外部登录。
        if (context?.IdP != null)
        {
            AuthenticationScheme? scheme = await schemeProvider.GetSchemeAsync(context.IdP);
            if (scheme != null)
            {
                bool isLocalIdP = context.IdP == IdentityServerConstants.LocalIdentityProvider;
                // this is meant to short circuit the UI and only trigger the one external IdP
                Model = new LoginOptionsModel
                {
                    EnableLocalLogin = isLocalIdP
                };

                Mobile = context.LoginHint ?? "";

                if (!isLocalIdP)
                    Model.ExternalProviders =
                    [
                        new ExternalLoginProvider
                        {
                            AuthenticationScheme = context.IdP,
                            DisplayName = scheme.DisplayName!
                        }
                    ];

                return;
            }

        }

        List<ExternalLoginProvider> externalProviders = [];
        // 载入所有静态登记的身份验证方案。
        IEnumerable<AuthenticationScheme> schemes = await schemeProvider.GetAllSchemesAsync();

        externalProviders.AddRange(schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalLoginProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }));

        //从IdentityServer存储加载验证方案。
        IEnumerable<ExternalLoginProvider> dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ExternalLoginProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName ?? ""
            });

        externalProviders.AddRange(dynamicSchemes);


        var allowLocal = true;
        Client? client = context?.Client;
        List<ExternalLoginProvider> allowExternalProviders = externalProviders;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin; //客户端是否允许本地登录
            if (client.IdentityProviderRestrictions.Count != 0)
            {
                //过滤只有客户端允许的登录提供器。
                allowExternalProviders = externalProviders.Where(p =>
                    client.IdentityProviderRestrictions.Contains(p.AuthenticationScheme)).ToList();
            }
        }

        Model = new LoginOptionsModel
        {
            AllowRememberLogin = loginOptions.Value.AllowRememberLogin,
            EnableLocalLogin = allowLocal && loginOptions.Value.AllowLocalLogin,
            ExternalProviders = [.. allowExternalProviders]
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

    public class InputModel
    {

        [Display(Name = "Remember me on this device")]
        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; }

        public string Button { get; set; } = null!;
    }

    public class LoginOptionsModel
    {
        /// <summary>
        /// 获取或设置是否允许记住登录。
        /// </summary>
        public bool AllowRememberLogin { get; set; } = true;

        /// <summary>
        /// 获取或设置是否启用本地登录。
        /// </summary>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// 获取所有外部登录提供器。
        /// </summary>
        public IEnumerable<ExternalLoginProvider> ExternalProviders { get; set; } = [];

        /// <summary>
        /// 获取可见的外部登录提供器。
        /// </summary>
        public IEnumerable<ExternalLoginProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        /// <summary>
        /// 只是是否仅外部登录。当且仅当禁用本地登录，且外部登录提供器只有1个时，该属性为true。
        /// </summary>
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders.Count() == 1;

        public string? ExternalLoginScheme =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;

        public string? ExternalLoginDisplayName =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.DisplayName : null;
    }

    public class ExternalLoginProvider
    {
        public string DisplayName { get; set; } = null!;
        public string AuthenticationScheme { get; set; } = null!;
    }
}