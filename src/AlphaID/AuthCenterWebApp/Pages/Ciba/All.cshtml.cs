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
    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; } = null!;

    [BindProperty]
    [Required]
    public string Id { get; set; } = null!;

    [BindProperty]
    [Required]
    public string Button { get; set; } = null!;

    public async Task OnGet()
    {
        Logins = await backchannelAuthenticationInteractionService.GetPendingLoginRequestsForCurrentUserAsync();
    }
}