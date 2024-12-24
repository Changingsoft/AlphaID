using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class IndexModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Client { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? OperationMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int anchor)
    {
        Client? data = await GetClient(anchor);
        if (data == null)
            return NotFound();
        Client = data;
        Input = new InputModel
        {
            ClientId = Client.ClientId,
            ClientName = Client.ClientName,
            Description = Client.Description,
            Enabled = Client.Enabled,
            ClientUri = Client.ClientUri,
            LogoUri = Client.LogoUri,
            RequireClientSecret = Client.RequireClientSecret
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? data = await GetClient(anchor);
        if (data == null)
            return NotFound();
        Client = data;

        if (Input.ClientId != Client.ClientId && dbContext.Clients.Any(p => p.ClientId == Input.ClientId))
            ModelState.AddModelError("", "Client Anchor ÒÑ´æÔÚ¡£");

        if (!ModelState.IsValid)
            return Page();

        Client.ClientName = Input.ClientName;
        Client.ClientId = Input.ClientId;
        Client.Description = Input.Description;
        Client.Enabled = Input.Enabled;
        Client.LogoUri = Input.LogoUri;
        Client.ClientUri = Input.ClientUri;
        Client.RequireClientSecret = Input.RequireClientSecret;

        dbContext.Clients.Update(Client);
        await dbContext.SaveChangesAsync();
        OperationMessage = "Applied";
        return Page();
    }

    private Task<Client?> GetClient(int anchor)
    {
        return dbContext.Clients
            .Include(p => p.AllowedScopes)
            .Include(p => p.AllowedGrantTypes)
            .Include(p => p.RedirectUris)
            .Include(p => p.PostLogoutRedirectUris)
            .Include(p => p.AllowedCorsOrigins)
            .Include(p => p.IdentityProviderRestrictions)
            .Include(p => p.Properties)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == anchor);
    }

    public class InputModel
    {
        [Display(Name = "Client name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string ClientName { get; set; } = default!;

        [Display(Name = "Client ID")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(200, ErrorMessage = "Validate_StringLength")]
        public string ClientId { get; set; } = default!;

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Validate_StringLength")]
        public string? Description { get; set; }

        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Logo URI")]
        [DataType(DataType.Url)]
        public string? LogoUri { get; set; }

        [Display(Name = "Client URI")]
        [DataType(DataType.Url)]
        public string? ClientUri { get; set; }

        [Display(Name = "Require client secret")]
        public bool RequireClientSecret { get; set; }
    }
}