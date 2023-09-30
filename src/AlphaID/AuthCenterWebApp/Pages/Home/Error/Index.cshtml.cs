using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Home.Error;

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public ViewModel View { get; set; }

    public Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
    {
        this._interaction = interaction;
        this._environment = environment;
    }

    public async Task OnGet(string errorId)
    {
        this.View = new ViewModel();

        // retrieve error details from identityserver
        var message = await this._interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            this.View.Error = message;

            if (!this._environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }
    }
}