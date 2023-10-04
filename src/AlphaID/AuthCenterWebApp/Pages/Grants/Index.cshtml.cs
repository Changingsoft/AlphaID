using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Grants;

[Authorize]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clients;
    private readonly IResourceStore _resources;
    private readonly IEventService _events;

    public Index(IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources,
        IEventService events)
    {
        this._interaction = interaction;
        this._clients = clients;
        this._resources = resources;
        this._events = events;
    }

    public ViewModel View { get; set; }

    public async Task OnGet()
    {
        var grants = await this._interaction.GetAllUserGrantsAsync();

        var list = new List<GrantViewModel>();
        foreach (var grant in grants)
        {
            var client = await this._clients.FindClientByIdAsync(grant.ClientId);
            if (client != null)
            {
                var resources = await this._resources.FindResourcesByScopeAsync(grant.Scopes);

                var item = new GrantViewModel()
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName ?? client.ClientId,
                    ClientLogoUrl = client.LogoUri,
                    ClientUrl = client.ClientUri,
                    Description = grant.Description,
                    Created = grant.CreationTime,
                    Expires = grant.Expiration,
                    IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                    ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                };

                list.Add(item);
            }
        }

        this.View = new ViewModel
        {
            Grants = list
        };
    }

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    public string ClientId { get; set; }

    public async Task<IActionResult> OnPost()
    {
        await this._interaction.RevokeUserConsentAsync(this.ClientId);
        await this._events.RaiseAsync(new GrantsRevokedEvent(this.User.GetSubjectId(), this.ClientId));

        return this.RedirectToPage("/Grants/Index");
    }
}