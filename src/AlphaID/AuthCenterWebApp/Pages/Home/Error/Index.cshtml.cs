using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Home.Error;

[AllowAnonymous]
[SecurityHeaders]
public class Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment) : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task OnGet(string errorId)
    {
        this.View = new ViewModel();

        // retrieve error details from identity server
        var message = await interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            this.View.Error = message;

            if (!environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }
    }

    public class ViewModel
    {
        public ViewModel()
        {
        }

        public ViewModel(string error)
        {
            this.Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage? Error { get; set; }
    }
}