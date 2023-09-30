#nullable disable

using Duende.IdentityServer;
using IDSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthCenterWebApp.Pages.Account.Manage;

public class ExternalLoginsModel : PageModel
{
    private readonly NaturalPersonManager _userManager;
    private readonly SignInManager<NaturalPerson> _signInManager;
    private readonly IUserStore<NaturalPerson> _userStore;

    public ExternalLoginsModel(
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager,
        IUserStore<NaturalPerson> userStore)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._userStore = userStore;
    }

    public IList<UserLoginInfo> CurrentLogins { get; set; }

    public IList<AuthenticationScheme> OtherLogins { get; set; }

    public bool ShowRemoveButton { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        this.CurrentLogins = await this._userManager.GetLoginsAsync(user);
        this.OtherLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => this.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        string passwordHash = null;
        if (this._userStore is IUserPasswordStore<NaturalPerson> userPasswordStore)
        {
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, this.HttpContext.RequestAborted);
        }

        this.ShowRemoveButton = passwordHash != null || this.CurrentLogins.Count > 1;
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }

        var result = await this._userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            this.StatusMessage = "The external login was not removed.";
            return this.RedirectToPage();
        }

        await this._signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "已删除外部登录。";
        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        var redirectUrl = this.Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
        var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, this._userManager.GetUserId(this.User));
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
        var user = await this._userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
        }
        var userId = await this._userManager.GetUserIdAsync(user);
        var info = await this._signInManager.GetExternalLoginInfoAsync(userId) ?? throw new InvalidOperationException($"加载外部登录信息时发生意外错误。");
        var result = await this._userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            this.StatusMessage = "未添加外部登录名。外部登录只能与一个帐户关联。";
            return this.RedirectToPage();
        }

        // Clear the existing external cookie to ensure a clean login process
        await this.HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme); //修正以使用IdeneityServer的方案。

        this.StatusMessage = "已添加外部登录。";
        return this.RedirectToPage();
    }

    /// <summary>
    /// 参考<see cref="SignInManager{TUser}.GetExternalLoginInfoAsync(string)"/>方法的实现。该实现替换了外部登录方案的常量值，以使用IdentityServer给出的外部登录方案。
    /// 请参阅
    /// </summary>
    /// <param name="expectedXsrf"></param>
    /// <returns></returns>
    [Obsolete()]
    private async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
    {
        var auth = await this.HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme); //修正以使用IdeneityServer的方案。
        var items = auth?.Properties?.Items;
        if (auth?.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
        {
            return null;
        }

        if (expectedXsrf != null)
        {
            if (!items.ContainsKey(XsrfKey))
            {
                return null;
            }
            var userId = items[XsrfKey];
            if (userId != expectedXsrf)
            {
                return null;
            }
        }

        var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (providerKey == null || items[LoginProviderKey] is not string provider)
        {
            return null;
        }

        var providerDisplayName = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).FirstOrDefault(p => p.Name == provider)?.DisplayName
                                  ?? provider;
        return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
        {
            AuthenticationTokens = auth.Properties.GetTokens(),
            AuthenticationProperties = auth.Properties
        };
    }

    private const string LoginProviderKey = "LoginProvider";
    private const string XsrfKey = "XsrfId";
}
