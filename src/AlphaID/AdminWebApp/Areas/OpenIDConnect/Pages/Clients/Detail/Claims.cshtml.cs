using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class ClaimsModel(ConfigurationDbContext dbContext) : PageModel
{
    public Client Data { get; set; } = default!;

    [BindProperty]
    public AddClaimModel Input { get; set; } = default!;

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;
        return Page();
    }

    public async Task<IActionResult> OnPostAddClaimAsync(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        if (Data.Claims.Any(p => p.Type == Input.Type && p.Value == Input.Value))
            ModelState.AddModelError("", "已经存在一个具有相同类型和值的声明。");

        if (!ModelState.IsValid) return Page();

        Data.Claims.Add(new ClientClaim
        {
            Type = Input.Type,
            Value = Input.Value
        });
        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveClaimAsync(int anchor, int claimId)
    {
        Client? data = dbContext.Clients.Include(p => p.Claims).Include(p => p.PostLogoutRedirectUris)
            .FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        ClientClaim? item = Data.Claims.FirstOrDefault(p => p.Id == claimId);
        if (item != null)
        {
            Data.Claims.Remove(item);
            dbContext.Clients.Update(Data);
            await dbContext.SaveChangesAsync();
        }

        return Page();
    }

    public class AddClaimModel
    {
        [Display(Name = "Type")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(255, ErrorMessage = "Validate_StringLength")]
        public string Type { get; set; } = default!;

        [Display(Name = "Value")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(255, ErrorMessage = "Validate_StringLength")]
        public string Value { get; set; } = default!;
    }
}