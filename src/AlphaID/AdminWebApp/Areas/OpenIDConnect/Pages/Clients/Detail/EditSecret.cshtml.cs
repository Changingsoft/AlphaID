using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail
{
    public class EditSecretModel : PageModel
    {
        private readonly ConfigurationDbContext configurationDbContext;

        public EditSecretModel(ConfigurationDbContext configurationDbContext)
        {
            this.configurationDbContext = configurationDbContext;
        }

        public Client Client { get; set; } = default!;

        public ClientSecret Secret { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet(int id, int secretId)
        {
            var client = this.configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == id);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(id);
            this.Secret = secret;
            this.Input = new InputModel
            {
                Expires = this.Secret.Expiration,
                Description = this.Secret.Description,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, int secretId)
        {
            var client = this.configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == id);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(id);
            this.Secret = secret;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Secret.Expiration = this.Input.Expires;
            this.Secret.Description = this.Input.Description;
            this.configurationDbContext.Clients.Update(this.Client);
            await this.configurationDbContext.SaveChangesAsync();
            return this.RedirectToPage("Secrets", new { id });
        }

        public async Task<IActionResult> OnPostRemoveSecret(int id, int secretId)
        {
            var client = this.configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == id);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            var secret = client.ClientSecrets.FirstOrDefault(p => p.Id == secretId);
            if (secret == null)
                return this.NotFound(id);
            this.Secret = secret;

            this.Client.ClientSecrets.Remove(this.Secret);
            this.configurationDbContext.Clients.Update(this.Client);
            await this.configurationDbContext.SaveChangesAsync();
            return this.RedirectToPage("Secrets", new { id });
        }
        public class InputModel
        {
            [Display(Name = " ß–ß ±º‰")]
            public DateTime? Expires { get; set; }

            [Display(Name = "√Ë ˆ")]
            public string? Description { get; set; }
        }
    }
}
