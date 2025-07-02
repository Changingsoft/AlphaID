using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class NewSecretModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(int id)
    {
        ApiResource? data = dbContext.ApiResources.FirstOrDefault(p => p.Id == id);
        if (data == null)
            return NotFound();

        Input = new InputModel
        {
            Value = GeneratePassword(),
            Type = "SharedSecret"
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ApiResource? data = dbContext.ApiResources.Include(p => p.Secrets).FirstOrDefault(p => p.Id == id);
        if (data == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        data.Secrets.Add(new ApiResourceSecret
        {
            Description = Input.Description,
            Expiration = Input.Expires,
            Value = Input.Value.ToSha256(),
            Type = "SharedSecret",
            Created = DateTime.UtcNow
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
        public string Type { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Value { get; set; } = null!;

        public DateTime? Expires { get; set; }
    }
}