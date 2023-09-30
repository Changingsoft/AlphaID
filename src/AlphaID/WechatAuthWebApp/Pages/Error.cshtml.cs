using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace WechatAuthWebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string RequestId { get; set; } = default!;

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    public ErrorModel()
    {
    }

    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
