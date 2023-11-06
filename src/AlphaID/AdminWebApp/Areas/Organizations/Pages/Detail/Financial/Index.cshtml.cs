using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Financial;

public class IndexModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public IndexModel(OrganizationManager organizationManager)
    {
        this.organizationManager = organizationManager;
    }

    public GenericOrganization Data { get; set; } = default!;


    [BindProperty]
    public InputModel Input { get; set; } = default!;


    public async Task<IActionResult> OnGet(string anchor)
    {
        var data = await this.organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        return this.Page();
    }

    public IActionResult OnPost(string anchor)
    {
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        public string Name { get; init; } = default!;

        [Display(Name = "Taxpayer ID")]
        public string TaxpayerId { get; init; } = default!;

        [Display(Name = "Address")]
        public string Address { get; init; } = default!;

        [Display(Name = "Contact")]
        public string Contact { get; init; } = default!;

        [Display(Name = "Bank")]
        public string Bank { get; init; } = default!;

        [Display(Name = "Account")]
        public string Account { get; init; } = default!;
    }

}
