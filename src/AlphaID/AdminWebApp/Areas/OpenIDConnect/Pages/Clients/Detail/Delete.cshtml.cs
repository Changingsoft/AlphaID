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
            return NotFound();
        }
        Data = client;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        var client = dbContext.Clients.SingleOrDefault(p => p.Id == anchor);
        if (client == null)
        {
            return RedirectToPage("../Index");
        }
        Data = client;

        if (ClientName != client.ClientName)
            ModelState.AddModelError(nameof(ClientName), "输入的客户端名不匹配。");

        if (!ModelState.IsValid)
            return Page();

        dbContext.Clients.Remove(client);
        await dbContext.SaveChangesAsync();
        return RedirectToPage("../Index");

    }
}
