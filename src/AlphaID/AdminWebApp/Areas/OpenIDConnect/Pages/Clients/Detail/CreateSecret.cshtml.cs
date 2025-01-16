using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class CreateSecretModel(ConfigurationDbContext configurationDbContext) : PageModel
{
    public Client Client { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(int anchor)
    {
        Client? client = configurationDbContext.Clients.Include(p => p.ClientSecrets)
            .FirstOrDefault(p => p.Id == anchor);
        if (client == null) return NotFound();
        Client = client;
        Input = new InputModel
        {
            Secret = GeneratePassword()
        };
        return Page();
    }

    private static string GeneratePassword()
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[24];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? client = configurationDbContext.Clients.Include(p => p.ClientSecrets)
            .FirstOrDefault(p => p.Id == anchor);
        if (client == null) return NotFound();
        Client = client;

        if (!ModelState.IsValid) return Page();

        ClientSecret secret = new()
        {
            ClientId = Client.Id,
            Type = "SharedSecret",
            Value = Input.Secret.ToSha256(),
            Created = DateTime.UtcNow,
            Description = Input.Description
        };
        if (Input.Expires.HasValue)
            secret.Expiration = Input.Expires.Value;

        Client.ClientSecrets.Add(secret);
        await configurationDbContext.SaveChangesAsync();
        return RedirectToPage("Secrets", new { anchor });
    }

    public class InputModel
    {
        [Display(Name = "Secret")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Secret { get; set; } = null!;

        [Display(Name = "Expires")]
        public DateTime? Expires { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; internal set; }
    }
}