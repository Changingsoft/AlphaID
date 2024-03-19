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
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddCallbackUrlAsync(int anchor, string callbackUri)
    {
        callbackUri = callbackUri.Trim();
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        if (this.Data.RedirectUris.Any(p => p.RedirectUri == callbackUri))
        {
            return this.Page();
        }

        this.Data.RedirectUris.Add(new ClientRedirectUri()
        {
            RedirectUri = callbackUri,
            ClientId = this.Data.Id,
            Client = this.Data,
        });
        dbContext.Update(this.Data);
        await dbContext.SaveChangesAsync();
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveCallbackUrlAsync(int anchor, int rid)
    {
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        var uri = this.Data.RedirectUris.First(p => p.Id == rid);
        this.Data.RedirectUris.Remove(uri);
        dbContext.Clients.Update(this.Data);
        await dbContext.SaveChangesAsync();
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddSignOutCallbackUrlAsync(int anchor, string callbackUri)
    {
        callbackUri = callbackUri.Trim();
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        if (this.Data.PostLogoutRedirectUris.Any(p => p.PostLogoutRedirectUri == callbackUri))
        {
            return this.Page();
        }

        this.Data.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri()
        {
            PostLogoutRedirectUri = callbackUri,
            ClientId = this.Data.Id,
            Client = this.Data,
        });
        dbContext.Update(this.Data);
        await dbContext.SaveChangesAsync();
        return this.Page();
    }
    public async Task<IActionResult> OnPostRemoveSignOutCallbackUrlAsync(int anchor, int srid)
    {
        var data = dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        var uri = this.Data.PostLogoutRedirectUris.First(p => p.Id == srid);
        this.Data.PostLogoutRedirectUris.Remove(uri);
        dbContext.Clients.Update(this.Data);
        await dbContext.SaveChangesAsync();
        return this.Page();
    }
}
