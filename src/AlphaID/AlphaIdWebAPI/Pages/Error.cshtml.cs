using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AlphaIdWebAPI.Pages;

/// <summary>
/// 
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    /// <summary>
    /// 
    /// </summary>
    public string RequestId { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    /// <summary>
    /// 
    /// </summary>
    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
