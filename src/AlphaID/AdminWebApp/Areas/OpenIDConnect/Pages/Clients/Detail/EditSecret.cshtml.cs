using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class EditSecretModel(ConfigurationDbContext configurationDbContext) : PageModel
{
    public Client Client { get; set; } = null!;

    public ClientSecret Secret { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(int anchor, int secretId)
    {
        Client? client = configurationDbContext.Clients.Include(p => p.ClientSecrets)
            .FirstOrDefault(p => p.Id == anchor);
        if (client == null)
            return NotFound();
        Client = client;
        ClientSecret? secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
        if (secret == null)
            return NotFound(anchor);
        Secret = secret;
        Input = new InputModel
        {
            Expires = Secret.Expiration,
            Description = Secret.Description
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor, int secretId)
    {
        Client? client = configurationDbContext.Clients.Include(p => p.ClientSecrets)
            .FirstOrDefault(p => p.Id == anchor);
        if (client == null)
            return NotFound();
        Client = client;
        ClientSecret? secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
        if (secret == null)
            return NotFound(anchor);
        Secret = secret;

        if (!ModelState.IsValid)
            return Page();

        Secret.Expiration = Input.Expires;
        Secret.Description = Input.Description;
        configurationDbContext.Clients.Update(Client);
        await configurationDbContext.SaveChangesAsync();
        return RedirectToPage("Secrets", new { anchor });
    }

    public async Task<IActionResult> OnPostRemoveSecretAsync(int anchor, int secretId)
    {
        Client? client = configurationDbContext.Clients.Include(p => p.ClientSecrets)
            .FirstOrDefault(p => p.Id == anchor);
        if (client == null)
            return NotFound();
        Client = client;
        ClientSecret? secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
        if (secret == null)
            return NotFound(anchor);
        Secret = secret;

        Client.ClientSecrets.Remove(Secret);
        configurationDbContext.Clients.Update(Client);
        await configurationDbContext.SaveChangesAsync();
        return RedirectToPage("Secrets", new { anchor });
    }

    public class InputModel
    {
        [Display(Name = "Expires")]
        public DateTime? Expires { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}