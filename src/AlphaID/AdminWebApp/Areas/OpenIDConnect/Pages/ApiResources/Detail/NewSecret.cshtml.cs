using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class NewSecretModel : PageModel
    {
        private readonly ConfigurationDbContext dbContext;

        public NewSecretModel(ConfigurationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var data = this.dbContext.ApiResources.FirstOrDefault(p => p.Id == id);
            if (data == null)
                return this.NotFound();

            this.Input = new InputModel()
            {
                Value = this.GeneratePassword(),
                Type = "SharedSecret",
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var data = this.dbContext.ApiResources.Include(p => p.Secrets).FirstOrDefault(p => p.Id == id);
            if (data == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            data.Secrets.Add(new Duende.IdentityServer.EntityFramework.Entities.ApiResourceSecret
            {
                Description = this.Input.Description,
                Expiration = this.Input.Expires,
                Value = this.Input.Value.ToSha256(),
                Type = "SharedSecret",
                Created = DateTime.UtcNow,
            });
            this.dbContext.ApiResources.Update(data);
            await this.dbContext.SaveChangesAsync();
            return RedirectToPage("Secrets", new { id });
        }
        private string GeneratePassword()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[24];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        public class InputModel
        {
            public string Type { get; set; } = default!;

            public string Description { get; set; } = default!;

            public string Value { get; set; } = default!;

            public DateTime? Expires { get; set; }
        }
    }
}
