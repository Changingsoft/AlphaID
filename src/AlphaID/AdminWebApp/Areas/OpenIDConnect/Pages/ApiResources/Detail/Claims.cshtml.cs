using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.ApiResources.Detail;

public class ClaimsModel(ConfigurationDbContext dbContext) : PageModel
{
    [BindProperty]
    [Display(Name = "New claim type")]
    public string NewClaim { get; set; } = null!;

    public ApiResource Data { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.UserClaims)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.UserClaims)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        if (Data.UserClaims.Any(p => p.Type == NewClaim))
            ModelState.AddModelError(nameof(NewClaim), "指定的声明类型已存在");

        if (!ModelState.IsValid)
            return Page();

        Data.UserClaims.Add(new ApiResourceClaim
        {
            Type = NewClaim
        });
        dbContext.ApiResources.Update(Data);
        await dbContext.SaveChangesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id, int claimId)
    {
        ApiResource? resource = await dbContext.ApiResources
            .Include(p => p.UserClaims)
            .AsSingleQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
        if (resource == null) return NotFound();

        Data = resource;
        ApiResourceClaim? item = Data.UserClaims.FirstOrDefault(p => p.Id == claimId);
        if (item != null)
        {
            Data.UserClaims.Remove(item);
            dbContext.ApiResources.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }
}