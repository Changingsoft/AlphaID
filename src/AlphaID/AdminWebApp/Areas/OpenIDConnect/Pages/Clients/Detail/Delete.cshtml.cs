using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class DeleteModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public DeleteModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [BindProperty]
    public string ClientName { get; set; } = default!;

    public Client Data { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var client = this.dbContext.Clients.SingleOrDefault(p => p.Id == anchor);
        if (client == null)
        {
            return this.NotFound();
        }
        this.Data = client;

        return this.Page();
    }

    public async Task<IActionResult> OnPost(int anchor)
    {
        var client = this.dbContext.Clients.SingleOrDefault(p => p.Id == anchor);
        if (client == null)
        {
            return this.RedirectToPage("../Index");
        }
        this.Data = client;

        if (this.ClientName != client.ClientName)
            this.ModelState.AddModelError(nameof(this.ClientName), "输入的客户端名不匹配。");

        if (!this.ModelState.IsValid)
            return this.Page();

        this.dbContext.Clients.Remove(client);
        await this.dbContext.SaveChangesAsync();
        return this.RedirectToPage("../Index");

    }
}
