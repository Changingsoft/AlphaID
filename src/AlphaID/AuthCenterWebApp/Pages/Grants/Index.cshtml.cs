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
public class Index(IIdentityServerInteractionService interaction,
    IClientStore clients,
    IResourceStore resourceStore,
    IEventService events) : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task OnGet()
    {
        var grants = await interaction.GetAllUserGrantsAsync();

        var list = new List<GrantViewModel>();
        foreach (var grant in grants)
        {
            var client = await clients.FindClientByIdAsync(grant.ClientId);
            if (client != null)
            {
                var resources = await resourceStore.FindResourcesByScopeAsync(grant.Scopes);

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

        View = new ViewModel
        {
            Grants = list
        };
    }

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    public string ClientId { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        await interaction.RevokeUserConsentAsync(ClientId);
        await events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), ClientId));

        return RedirectToPage("/Grants/Index");
    }

    public class ViewModel
    {
        public IEnumerable<GrantViewModel> Grants { get; set; } = default!;
    }

    public class GrantViewModel
    {
        public string ClientId { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public IEnumerable<string> IdentityGrantNames { get; set; } = default!;
        public IEnumerable<string> ApiGrantNames { get; set; } = default!;
    }
}