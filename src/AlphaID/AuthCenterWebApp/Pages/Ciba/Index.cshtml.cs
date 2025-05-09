using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Ciba;

[AllowAnonymous]
[SecurityHeaders]
public class IndexModel(
    IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService,
    ILogger<IndexModel> logger) : PageModel
{
    public BackchannelUserLoginRequest LoginRequest { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        BackchannelUserLoginRequest? loginRequest =
            await backchannelAuthenticationInteractionService.GetLoginRequestByInternalIdAsync(id);
        if (loginRequest == null)
        {
            logger.LogWarning("Invalid backchannel login id {id}", id);
            return RedirectToPage("/Home/Error/LoginModel");
        }

        LoginRequest = loginRequest;
        return Page();
    }
}