using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail
{
    public class ConsentModel(ConfigurationDbContext configurationDbContext) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public Client Client { get; set; } = default!;

        public string? OperationMessage { get; set; }

        public IActionResult OnGet(int anchor)
        {
            var client = configurationDbContext.Clients.FirstOrDefault(p => p.Id == anchor);
            if (client == null)
                return this.NotFound();
            this.Client = client;
            this.Input = new InputModel
            {
                RequireConsent = this.Client.RequireConsent,
                AllowRememberConsent = this.Client.AllowRememberConsent,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int anchor)
        {
            var client = configurationDbContext.Clients.FirstOrDefault(p => p.Id == anchor);
            if (client == null)
                return this.NotFound();
            this.Client = client;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Client.RequireConsent = this.Input.RequireConsent;
            this.Client.AllowRememberConsent = this.Input.AllowRememberConsent;
            configurationDbContext.Clients.Update(this.Client);
            await configurationDbContext.SaveChangesAsync();
            this.OperationMessage = "Applied";
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Require consent")]
            public bool RequireConsent { get; set; }

            [Display(Name = "Allow remember consent")]
            public bool AllowRememberConsent { get; set; }
        }
    }
}
