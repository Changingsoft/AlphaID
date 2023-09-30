// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

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
    public BackchannelUserLoginRequest LoginRequest { get; set; }

    private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService, ILogger<IndexModel> logger)
    {
        this._backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
        this._logger = logger;
    }

    public async Task<IActionResult> OnGet(string id)
    {
        this.LoginRequest = await this._backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
        if (this.LoginRequest == null)
        {
            this._logger.LogWarning("Invalid backchannel login id {id}", id);
            return this.RedirectToPage("/Home/Error/LoginModel");
        }

        return this.Page();
    }
}