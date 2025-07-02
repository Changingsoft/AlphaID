using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Grants;

[Authorize]
public class Index(
    IIdentityServerInteractionService interaction,
    IClientStore clients,
    IResourceStore resourceStore,
    IEventService events) : PageModel
{
    public ViewModel View { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientId { get; set; } = null!;

    public async Task OnGet()
    {
        IEnumerable<Grant> grants = await interaction.GetAllUserGrantsAsync();

        var list = new List<GrantViewModel>();
        foreach (Grant grant in grants)
        {
            Client? client = await clients.FindClientByIdAsync(grant.ClientId);
            if (client != null)
            {
                Duende.IdentityServer.Models.Resources? resources =
                    await resourceStore.FindResourcesByScopeAsync(grant.Scopes);

                var item = new GrantViewModel
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

        View = new ViewModel
        {
            Grants = list
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await interaction.RevokeUserConsentAsync(ClientId);
        await events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), ClientId));

        return RedirectToPage("/Grants/Index");
    }

    public class ViewModel
    {
        public IEnumerable<GrantViewModel> Grants { get; set; } = null!;
    }

    public class GrantViewModel
    {
        public string ClientId { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public IEnumerable<string> IdentityGrantNames { get; set; } = null!;
        public IEnumerable<string> ApiGrantNames { get; set; } = null!;
    }
}