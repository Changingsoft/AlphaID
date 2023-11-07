using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using IDSubjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly SignInManager<NaturalPerson> signInManager;
    private readonly IIdentityServerInteractionService interaction;
    private readonly IEventService events;

    [BindProperty]
    public string? LogoutId { get; set; }

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public Index(SignInManager<NaturalPerson> signInManager, IIdentityServerInteractionService interaction, IEventService events)
    {
        this.signInManager = signInManager;
        this.interaction = interaction;
        this.events = events;
    }

    public async Task<IActionResult> OnGet(string? logoutId, string? returnUrl)
    {
        this.LogoutId = logoutId;
        this.ReturnUrl = returnUrl;

        var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        if (this.User.Identity!.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            var context = await this.interaction.GetLogoutContextAsync(this.LogoutId);
            if (context.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showLogoutPrompt = false;
            }
        }

        if (showLogoutPrompt == false)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await this.OnPost();
        }

        return this.Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (this.User.Identity!.IsAuthenticated)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed
            this.LogoutId ??= await this.interaction.CreateLogoutContextAsync();

            // delete local authentication cookie
            await this.signInManager.SignOutAsync();

            // raise the logout event
            await this.events.RaiseAsync(new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));

            // see if we need to trigger federated logout
            var idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp is not null and not Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await this.HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string? url = this.Url.Page("/Account/Logout/Loggedout", new { logoutId = this.LogoutId });

                    // this triggers a redirect to the external provider for sign-out
                    return this.SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }
        }

        return this.RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = this.LogoutId, returnUrl = this.ReturnUrl });
    }
}