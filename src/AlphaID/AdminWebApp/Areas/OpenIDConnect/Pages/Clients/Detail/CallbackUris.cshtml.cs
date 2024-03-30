using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class CallbackUrisModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        return Page();
    }

    public async Task<IActionResult> OnPostAddCallbackUrlAsync(int anchor, string callbackUri)
    {
        callbackUri = callbackUri.Trim();
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        if (Data.RedirectUris.Any(p => p.RedirectUri == callbackUri))
        {
            return Page();
        }

        Data.RedirectUris.Add(new ClientRedirectUri()
        {
            RedirectUri = callbackUri,
            ClientId = Data.Id,
            Client = Data,
        });
        dbContext.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveCallbackUrlAsync(int anchor, int rid)
    {
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        var uri = Data.RedirectUris.First(p => p.Id == rid);
        Data.RedirectUris.Remove(uri);
        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAddSignOutCallbackUrlAsync(int anchor, string callbackUri)
    {
        callbackUri = callbackUri.Trim();
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        if (Data.PostLogoutRedirectUris.Any(p => p.PostLogoutRedirectUri == callbackUri))
        {
            return Page();
        }

        Data.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri()
        {
            PostLogoutRedirectUri = callbackUri,
            ClientId = Data.Id,
            Client = Data,
        });
        dbContext.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }
    public async Task<IActionResult> OnPostRemoveSignOutCallbackUrlAsync(int anchor, int srid)
    {
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        var uri = Data.PostLogoutRedirectUris.First(p => p.Id == srid);
        Data.PostLogoutRedirectUris.Remove(uri);
        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }
}
