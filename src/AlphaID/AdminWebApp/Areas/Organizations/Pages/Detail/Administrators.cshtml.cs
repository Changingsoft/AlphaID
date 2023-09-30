using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class AdministratorsModel : PageModel
{
    private readonly OrganizationManager manager;

    public AdministratorsModel(OrganizationManager manager)
    {
        this.manager = manager;
    }

    public GenericOrganization Organization { get; set; } = default!;

    public IEnumerable<OrganizationAdministrator> Administrators { get; set; } = default!;


    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        var organization = await this.manager.FindByIdAsync(id);
        if (organization == null)
        {
            return this.NotFound();
        }
        this.Organization = organization;
        this.Administrators = await this.manager.GetAdministratorsAsync(organization);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddAdministrator(string id)
    {
        var organization = await this.manager.FindByIdAsync(id);
        if (organization == null)
        {
            return this.NotFound();
        }
        this.Administrators = await this.manager.GetAdministratorsAsync(organization);

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var result = await this.manager.AddAdministratorAsync(organization, new PersonInfo(this.Input.PersonId, this.Input.Name, this.Input.PhoneticSearchHint));
        if (result.IsSuccess)
        {
            return this.RedirectToPage(new { id });
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error);
        }
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAdministrator(string id, string administratorId)
    {
        var organization = await this.manager.FindByIdAsync(id);
        if (organization == null)
        {
            return this.NotFound();
        }
        this.Administrators = await this.manager.GetAdministratorsAsync(organization);

        var administrator = this.Administrators.FirstOrDefault(p => p.PersonId == administratorId);
        if (administrator != null)
        {
            var result = await this.manager.RemoveAdministratorAsync(organization, administrator);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError("", error);
                }
                return this.Page();
            }
        }
        return this.RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostSetCreator(string id, string administratorId)
    {
        var organization = await this.manager.FindByIdAsync(id);
        if (organization == null)
        {
            return this.NotFound();
        }
        this.Administrators = await this.manager.GetAdministratorsAsync(organization);

        var administrator = this.Administrators.FirstOrDefault(p => p.PersonId == administratorId);
        if (administrator == null)
        {
            this.ModelState.AddModelError("", "找不到指定的组织管理人");
            return this.Page();
        }

        var result = await this.manager.SetCreatorAsync(organization, administrator);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        return this.RedirectToPage(new { id });
    }

    public class InputModel
    {
        public string PersonId { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? PhoneticSearchHint { get; set; }
    }
}
