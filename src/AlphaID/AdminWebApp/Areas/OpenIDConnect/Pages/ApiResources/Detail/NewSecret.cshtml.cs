using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail
{
    public class NewSecretModel(ConfigurationDbContext dbContext) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var data = dbContext.ApiResources.FirstOrDefault(p => p.Id == id);
            if (data == null)
                return NotFound();

            Input = new InputModel()
            {
                Value = GeneratePassword(),
                Type = "SharedSecret",
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var data = dbContext.ApiResources.Include(p => p.Secrets).FirstOrDefault(p => p.Id == id);
            if (data == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            data.Secrets.Add(new Duende.IdentityServer.EntityFramework.Entities.ApiResourceSecret
            {
                Description = Input.Description,
                Expiration = Input.Expires,
                Value = Input.Value.ToSha256(),
                Type = "SharedSecret",
                Created = DateTime.UtcNow,
            });
            dbContext.ApiResources.Update(data);
            await dbContext.SaveChangesAsync();
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
