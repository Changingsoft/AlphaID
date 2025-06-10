using AlphaIdPlatform.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class Index(
    SignInManager<NaturalPerson> signInManager,
    IIdentityServerInteractionService interaction,
    IEventService events) : PageModel
{
    [BindProperty]
    public string? LogoutId { get; set; }

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string? logoutId, string? returnUrl)
    {
        LogoutId = logoutId;
        ReturnUrl = returnUrl;

        bool showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        if (User.Identity!.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            LogoutRequest context = await interaction.GetLogoutContextAsync(LogoutId);
            if (context.ShowSignoutPrompt == false)
                // it's safe to automatically sign-out
                showLogoutPrompt = false;
        }

        if (showLogoutPrompt == false)
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await OnPostAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity!.IsAuthenticated)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged-in user
            // this can still return null if there is no context needed
            LogoutId ??= await interaction.CreateLogoutContextAsync();

            // delete local authentication cookie
            await signInManager.SignOutAsync();

            // raise the logout event
            await events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // see if we need to trigger federated logout
            string? idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp is not null and not IdentityServerConstants.LocalIdentityProvider)
                // we need to see if the provider supports external logout
                if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string? url = Url.Page("/Account/Logout/LoggedOut", new { logoutId = LogoutId });

                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
        }

        return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId, returnUrl = ReturnUrl });
    }
}