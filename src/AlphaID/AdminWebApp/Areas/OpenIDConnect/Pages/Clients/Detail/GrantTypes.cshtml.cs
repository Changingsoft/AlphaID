using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class GrantTypesModel : PageModel
{
    private readonly ConfigurationDbContext dbContext;

    public GrantTypesModel(ConfigurationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Duende.IdentityServer.EntityFramework.Entities.Client Data { get; set; } = default!;

    [BindProperty]
    public List<SelectListItem> AllowedGrantTypes { get; set; } = default!;

    public string? OperationMessage { get; set; }

    public IActionResult OnGet(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedGrantTypes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        this.AllowedGrantTypes = GrantTypes.Select(g => new SelectListItem(g.DisplayName, g.Name, this.Data.AllowedGrantTypes.Any(p => p.GrantType == g.Name))).ToList();

        return this.Page();
    }

    public async Task<IActionResult> OnPost(int anchor)
    {
        var data = this.dbContext.Clients.Include(p => p.AllowedGrantTypes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;

        var selectedGrantTypes = this.AllowedGrantTypes.Where(p => p.Selected).Select(p => p.Value);
        try
        {
            //Check any grant type combination is available.
            Duende.IdentityServer.Models.Client.ValidateGrantTypes(selectedGrantTypes);
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }

        foreach (var grantType in this.AllowedGrantTypes)
        {
            if (grantType.Selected)
            {
                if (!this.Data.AllowedGrantTypes.Any(p => p.GrantType == grantType.Value))
                {
                    this.Data.AllowedGrantTypes.Add(new ClientGrantType()
                    {
                        ClientId = this.Data.Id,
                        GrantType = grantType.Value,
                    });
                }
            }
            else
            {
                var existsGrantType = this.Data.AllowedGrantTypes.FirstOrDefault(p => p.GrantType == grantType.Value);
                if (existsGrantType != null)
                {
                    this.Data.AllowedGrantTypes.Remove(existsGrantType);
                }
            }
        }
        this.dbContext.Clients.Update(this.Data);
        await this.dbContext.SaveChangesAsync();
        this.OperationMessage = "操作已成功！";
        return this.Page();
    }

    private static readonly List<GrantTypeItem> GrantTypes = new()
    {
        new GrantTypeItem(GrantType.Implicit, "隐式"),
        new GrantTypeItem(GrantType.AuthorizationCode, "授权码"),
        new GrantTypeItem(GrantType.ClientCredentials, "客户端凭证"),
        new GrantTypeItem(GrantType.ResourceOwnerPassword, "资源所有者密码"),
        new GrantTypeItem(GrantType.DeviceFlow, "设备码"),
        new GrantTypeItem(GrantType.Hybrid, "混合"),
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DisplayName"></param>
    /// <param name="Description"></param>
    public record GrantTypeItem(string Name, string DisplayName, string? Description = null);
}
