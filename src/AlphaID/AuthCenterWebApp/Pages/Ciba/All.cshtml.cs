using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Ciba;

[Authorize]
public class AllModel : PageModel
{
    private readonly IBackchannelAuthenticationInteractionService backchannelAuthenticationInteraction;
    public AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
    {
        this.backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
    }

    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; } = default!;

    [BindProperty, Required]
    public string Id { get; set; } = default!;
    [BindProperty, Required]
    public string Button { get; set; } = default!;

    public async Task OnGet()
    {
        this.Logins = await this.backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
    }
}