using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Ciba;

[Authorize]
public class AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
    : PageModel
{
    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; } = default!;

    [BindProperty]
    [Required]
    public string Id { get; set; } = default!;

    [BindProperty]
    [Required]
    public string Button { get; set; } = default!;

    public async Task OnGet()
    {
        Logins = await backchannelAuthenticationInteractionService.GetPendingLoginRequestsForCurrentUserAsync();
    }
}