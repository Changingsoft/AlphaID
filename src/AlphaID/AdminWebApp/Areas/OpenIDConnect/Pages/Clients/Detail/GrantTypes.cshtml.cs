using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client = Duende.IdentityServer.EntityFramework.Entities.Client;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients.Detail;

public class GrantTypesModel(ConfigurationDbContext dbContext) : PageModel
{
    private static readonly List<GrantTypeItem> s_grantTypes =
    [
        new GrantTypeItem(GrantType.Implicit, "隐式"),
        new GrantTypeItem(GrantType.AuthorizationCode, "授权码"),
        new GrantTypeItem(GrantType.ClientCredentials, "客户端凭证"),
        new GrantTypeItem(GrantType.ResourceOwnerPassword, "资源所有者密码"),
        new GrantTypeItem(GrantType.DeviceFlow, "设备码"),
        new GrantTypeItem(GrantType.Hybrid, "混合")
    ];

    public Client Data { get; set; } = default!;

    [BindProperty]
    public List<SelectListItem> AllowedGrantTypes { get; set; } = default!;

    public string? OperationMessage { get; set; }

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedGrantTypes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        AllowedGrantTypes = s_grantTypes.Select(g =>
            new SelectListItem(g.DisplayName, g.Name, Data.AllowedGrantTypes.Any(p => p.GrantType == g.Name))).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedGrantTypes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        IEnumerable<string> selectedGrantTypes = AllowedGrantTypes.Where(p => p.Selected).Select(p => p.Value);
        try
        {
            //Check any grant type combination is available.
            Duende.IdentityServer.Models.Client.ValidateGrantTypes(selectedGrantTypes);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }

        foreach (SelectListItem grantType in AllowedGrantTypes)
            if (grantType.Selected)
            {
                if (!Data.AllowedGrantTypes.Any(p => p.GrantType == grantType.Value))
                    Data.AllowedGrantTypes.Add(new ClientGrantType
                    {
                        ClientId = Data.Id,
                        GrantType = grantType.Value
                    });
            }
            else
            {
                ClientGrantType? existsGrantType =
                    Data.AllowedGrantTypes.FirstOrDefault(p => p.GrantType == grantType.Value);
                if (existsGrantType != null) Data.AllowedGrantTypes.Remove(existsGrantType);
            }

        dbContext.Clients.Update(Data);
        await dbContext.SaveChangesAsync();
        OperationMessage = "操作已成功！";
        return Page();
    }

    /// <summary>
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DisplayName"></param>
    /// <param name="Description"></param>
    public record GrantTypeItem(string Name, string DisplayName, string? Description = null);
}