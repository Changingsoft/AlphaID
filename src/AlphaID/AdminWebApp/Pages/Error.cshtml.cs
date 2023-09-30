using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminWebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string RequestId { get; set; } = default!;

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
