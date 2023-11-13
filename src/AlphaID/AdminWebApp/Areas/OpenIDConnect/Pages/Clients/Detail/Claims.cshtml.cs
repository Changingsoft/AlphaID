using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class ClaimsModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public ClaimsModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Client Data { get; set; } = default!;

    [BindProperty]
    public AddClaimModel Input { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddClaimAsync(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        if (this.Data.Claims.Any(p => p.Type == this.Input.Type && p.Value == this.Input.Value))
        {
            this.ModelState.AddModelError("", "已经存在一个具有相同类型和值的声明。");
        }

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        this.Data.Claims.Add(new ClientClaim()
        {
            Type = this.Input.Type,
            Value = this.Input.Value,
        });
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();

        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveClaimAsync(int anchor, int claimId)
    {
        var data = this.dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        var item = this.Data.Claims.FirstOrDefault(p => p.Id == claimId);
        if (item != null)
        {
            this.Data.Claims.Remove(item);
            this.dbContext.Clients.Update(this.Data);
            await this.dbContext.SaveChangesAsync();
        }
        return this.Page();
    }

    public class AddClaimModel
    {
        [Display(Name = "Type")]
        [StringLength(255, ErrorMessage = "Validate_StringLength")]
        public string Type { get; set; } = default!;

        [Display(Name = "Type")]
        [StringLength(255, ErrorMessage = "Validate_StringLength")]
        public string Value { get; set; } = default!;
    }
}
