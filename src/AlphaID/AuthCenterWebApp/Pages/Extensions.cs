using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages;

public static class Extensions
{
    /// <summary>
    ///     ȷ�������֤�����Ƿ�֧��ע����
    /// </summary>
    public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
    {
        var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        IAuthenticationHandler? handler = await provider.GetHandlerAsync(context, scheme);
        return handler is IAuthenticationSignOutHandler;
    }

    /// <summary>
    ///     ����ض��� URI �Ƿ������ڱ����ͻ��ˡ�
    /// </summary>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    ///     ���������ض�����ض��� URI �ļ���ҳ��
    /// </summary>
    public static IActionResult LoadingPage(this PageModel page, string redirectUri)
    {
        page.HttpContext.Response.StatusCode = 200;
        page.HttpContext.Response.Headers.Location = "";

        return page.RedirectToPage("/Redirect/LoginModel", new { RedirectUri = redirectUri });
    }
}