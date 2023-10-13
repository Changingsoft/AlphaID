using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail
{
    public class CreateSecretModel : PageModel
    {
        private readonly ConfigurationDbContext configurationDbContext;

        public CreateSecretModel(ConfigurationDbContext configurationDbContext)
        {
            this.configurationDbContext = configurationDbContext;
        }

        public Client Client { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var client = this.configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == id);
            if (client == null)
            {
                return this.NotFound();
            }
            this.Client = client;
            this.Input = new InputModel
            {
                Secret = this.GeneratePassword(),
            };
            return this.Page();
        }

        private string GeneratePassword()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[24];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = this.configurationDbContext.Clients.Include(p => p.ClientSecrets).FirstOrDefault(p => p.Id == id);
            if (client == null)
            {
                return this.NotFound();
            }
            this.Client = client;

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            ClientSecret secret = new()
            {
                ClientId = this.Client.Id,
                Type = "SharedSecret",
                Value = this.Input.Secret.ToSha256(),
                Created = DateTime.UtcNow,
                Description = this.Input.Description,
            };
            if (this.Input.Expires.HasValue)
                secret.Expiration = this.Input.Expires.Value;

            this.Client.ClientSecrets.Add(secret);
            await this.configurationDbContext.SaveChangesAsync();
            return this.RedirectToPage("Secrets", new { id });
        }

        public class InputModel
        {
            [Display(Name = "密钥值")]
            public string Secret { get; set; } = default!;

            [Display(Name = "失效时间")]
            public DateTime? Expires { get; set; }

            [Display(Name = "描述")]
            public string? Description { get; internal set; }
        }
    }
}
