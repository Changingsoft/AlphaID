using AlphaIdPlatform.Identity;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class ExternalLoginsModel(
    UserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    IUserStore<NaturalPerson> userStore) : PageModel
{
    private const string LoginProviderKey = "LoginProvider";
    private const string XsrfKey = "XsrfId";

    public IList<UserLoginInfo> CurrentLogins { get; set; } = [];

    public IList<AuthenticationScheme> OtherLogins { get; set; } = [];

    public bool ShowRemoveButton { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        CurrentLogins = await userManager.GetLoginsAsync(user);
        OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        string? passwordHash = null;
        if (userStore is IUserPasswordStore<NaturalPerson> userPasswordStore)
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);

        ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        IdentityResult result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            StatusMessage = "The external login was not removed.";
            return RedirectToPage();
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "已删除外部登录。";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // Request a redirect to the external login provider to link a login for the current user
        string? redirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
        AuthenticationProperties properties =
            signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        string userId = await userManager.GetUserIdAsync(user);
        ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync(userId) ??
                                 throw new InvalidOperationException("加载外部登录信息时发生意外错误。");
        IdentityResult result = await userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            StatusMessage = "未添加外部登录名。外部登录只能与一个帐户关联。";
            return RedirectToPage();
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityServerConstants
            .ExternalCookieAuthenticationScheme); //修正以使用IdentityServer的方案。

        StatusMessage = "已添加外部登录。";
        return RedirectToPage();
    }

    /// <summary>
    /// 参考<see cref="SignInManager{TUser}.GetExternalLoginInfoAsync(string)" />
    /// 方法的实现。该实现替换了外部登录方案的常量值，以使用IdentityServer给出的外部登录方案。
    /// 请参阅
    /// </summary>
    /// <param name="expectedXsrf"></param>
    /// <returns></returns>
    [Obsolete]
    private async Task<ExternalLoginInfo?> GetExternalLoginInfoAsync(string? expectedXsrf = null)
    {
        AuthenticateResult auth =
            await HttpContext.AuthenticateAsync(IdentityServerConstants
                .ExternalCookieAuthenticationScheme); //修正以使用Identity Server的方案。
        IDictionary<string, string?>? items = auth.Properties?.Items;
        if (auth.Principal == null || items == null || !items.ContainsKey(LoginProviderKey)) return null;

        if (expectedXsrf != null)
        {
            if (!items.TryGetValue(XsrfKey, out string? userId)) return null;
            if (userId != expectedXsrf) return null;
        }

        string? providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (providerKey == null || items[LoginProviderKey] is not { } provider) return null;

        string providerDisplayName = (await signInManager.GetExternalAuthenticationSchemesAsync())
                                     .FirstOrDefault(p => p.Name == provider)?.DisplayName
                                     ?? provider;
        return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
        {
            AuthenticationTokens = auth.Properties?.GetTokens(),
            AuthenticationProperties = auth.Properties
        };
    }
}