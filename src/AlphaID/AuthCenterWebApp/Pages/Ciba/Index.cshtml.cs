using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Ciba;

[AllowAnonymous]
[SecurityHeaders]
public class IndexModel : PageModel
{
    public BackchannelUserLoginRequest? LoginRequest { get; set; }

    private readonly IBackchannelAuthenticationInteractionService backchannelAuthenticationInteraction;
    private readonly ILogger<IndexModel> logger;

    public IndexModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService, ILogger<IndexModel> logger)
    {
        this.backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
        this.logger = logger;
    }

    public async Task<IActionResult> OnGet(string id)
    {
        this.LoginRequest = await this.backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
        if (this.LoginRequest == null)
        {
            this.logger.LogWarning("Invalid backchannel login id {id}", id);
            return this.RedirectToPage("/Home/Error/LoginModel");
        }

        return this.Page();
    }
}