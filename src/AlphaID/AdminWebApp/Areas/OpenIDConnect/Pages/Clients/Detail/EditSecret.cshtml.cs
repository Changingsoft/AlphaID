using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail
{
    public class EditSecretModel(ConfigurationDbContext configurationDbContext) : PageModel
    {
        public Client Client { get; set; } = default!;

        public ClientSecret Secret { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet(int anchor, int secretId)
        {
            var client = configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == anchor);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(anchor);
            this.Secret = secret;
            this.Input = new InputModel
            {
                Expires = this.Secret.Expiration,
                Description = this.Secret.Description,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int anchor, int secretId)
        {
            var client = configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == anchor);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(anchor);
            this.Secret = secret;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Secret.Expiration = this.Input.Expires;
            this.Secret.Description = this.Input.Description;
            configurationDbContext.Clients.Update(this.Client);
            await configurationDbContext.SaveChangesAsync();
            return this.RedirectToPage("Secrets", new { anchor });
        }

        public async Task<IActionResult> OnPostRemoveSecretAsync(int anchor, int secretId)
        {
            var client = configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == anchor);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(anchor);
            this.Secret = secret;

            this.Client.ClientSecrets.Remove(this.Secret);
            configurationDbContext.Clients.Update(this.Client);
            await configurationDbContext.SaveChangesAsync();
            return this.RedirectToPage("Secrets", new { anchor });
        }
        public class InputModel
        {
            [Display(Name = "Expires")]
            public DateTime? Expires { get; set; }

            [Display(Name = "Description")]
            public string? Description { get; set; }
        }
    }
}
