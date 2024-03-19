using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class DeleteModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    [Display(Name = "Client name")]
    public string ClientName { get; set; } = default!;

    public Client Data { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var client = dbContext.Clients.SingleOrDefault(p => p.Id == anchor);
        if (client == null)
        {
            return this.NotFound();
        }
        this.Data = client;

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        var client = dbContext.Clients.SingleOrDefault(p => p.Id == anchor);
        if (client == null)
        {
            return this.RedirectToPage("../Index");
        }
        this.Data = client;

        if (this.ClientName != client.ClientName)
            this.ModelState.AddModelError(nameof(this.ClientName), "输入的客户端名不匹配。");

        if (!this.ModelState.IsValid)
            return this.Page();

        dbContext.Clients.Remove(client);
        await dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");

    }
}
