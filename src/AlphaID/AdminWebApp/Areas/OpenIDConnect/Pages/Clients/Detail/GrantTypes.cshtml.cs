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
    public Client Data { get; set; } = null!;

    [BindProperty]
    public List<SelectListItem> AllowedGrantTypes { get; set; } = null!;

    public string? OperationMessage { get; set; }

    public IActionResult OnGet(int anchor)
    {
        Client? data = dbContext.Clients.Include(p => p.AllowedGrantTypes).FirstOrDefault(p => p.Id == anchor);
        if (data == null)
            return NotFound();
        Data = data;

        AllowedGrantTypes = ClientConstants.GrantTypes.Select(g =>
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

    
}