using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.New
{
    [BindProperties]
    public class PublicModel(ConfigurationDbContext dbContext) : PageModel
    {
        public string ClientId { get; set; } = default!;

        public string ClientName { get; set; } = default!;

        public string SigninCallbackUri { get; set; } = default!;

        //todo Identity resources and scope selected.

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            //todo create client and stored to database.
            if (!this.ModelState.IsValid)
                return this.Page();



            return this.RedirectToPage("../Detail/Index", new { anchor = ClientId });
        }
    }
}
