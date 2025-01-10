using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class ConsentModel(ConfigurationDbContext configurationDbContext) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public Client Client { get; set; } = null!;

    public string? OperationMessage { get; set; }

    public IActionResult OnGet(int anchor)
    {
        Client? client = configurationDbContext.Clients.FirstOrDefault(p => p.Id == anchor);
        if (client == null)
            return NotFound();
        Client = client;
        Input = new InputModel
        {
            RequireConsent = Client.RequireConsent,
            AllowRememberConsent = Client.AllowRememberConsent
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? client = configurationDbContext.Clients.FirstOrDefault(p => p.Id == anchor);
        if (client == null)
            return NotFound();
        Client = client;

        if (!ModelState.IsValid)
            return Page();

        Client.RequireConsent = Input.RequireConsent;
        Client.AllowRememberConsent = Input.AllowRememberConsent;
        configurationDbContext.Clients.Update(Client);
        await configurationDbContext.SaveChangesAsync();
        OperationMessage = "Applied";
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Require consent")]
        public bool RequireConsent { get; set; }

        [Display(Name = "Allow remember consent")]
        public bool AllowRememberConsent { get; set; }
    }
}