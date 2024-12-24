using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages;

public static class Extensions
{
    /// <summary>
    ///     确定身份验证方案是否支持注销。
    /// </summary>
    public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
    {
        var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        IAuthenticationHandler? handler = await provider.GetHandlerAsync(context, scheme);
        return handler is IAuthenticationSignOutHandler;
    }

    /// <summary>
    ///     检查重定向 URI 是否适用于本机客户端。
    /// </summary>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    ///     呈现用于重定向回重定向 URI 的加载页。
    /// </summary>
    public static IActionResult LoadingPage(this PageModel page, string redirectUri)
    {
        page.HttpContext.Response.StatusCode = 200;
        page.HttpContext.Response.Headers.Location = "";

        return page.RedirectToPage("/Redirect/LoginModel", new { RedirectUri = redirectUri });
    }
}