using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class CallbackUrisModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public CallbackUrisModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Client Data { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddCallbackUrlAsync(int anchor, string callback_uri)
    {
        callback_uri = callback_uri.Trim();
        var data = this.dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        if (!this.Data.RedirectUris.Any(p => p.RedirectUri == callback_uri))
        {
            this.Data.RedirectUris.Add(new ClientRedirectUri()
            {
                RedirectUri = callback_uri,
                ClientId = this.Data.Id,
                Client = this.Data,
            });
            this.dbContext.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveCallbackUrlAsync(int anchor, int rid)
    {
        var data = this.dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        var uri = this.Data.RedirectUris.First(p => p.Id == rid);
        this.Data.RedirectUris.Remove(uri);
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddSignOutCallbackUrlAsync(int anchor, string callback_uri)
    {
        callback_uri = callback_uri.Trim();
        var data = this.dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        if (!this.Data.PostLogoutRedirectUris.Any(p => p.PostLogoutRedirectUri == callback_uri))
        {
            this.Data.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri()
            {
                PostLogoutRedirectUri = callback_uri,
                ClientId = this.Data.Id,
                Client = this.Data,
            });
            this.dbContext.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
        }
        return this.Page();
    }
    public async Task<IActionResult> OnPostRemoveSignOutCallbackUrlAsync(int anchor, int srid)
    {
        var data = this.dbContext.Clients.Include(p => p.RedirectUris).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        var uri = this.Data.PostLogoutRedirectUris.First(p => p.Id == srid);
        this.Data.PostLogoutRedirectUris.Remove(uri);
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        return this.Page();
    }
}
