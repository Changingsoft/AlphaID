using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class FinancialModel : PageModel
{
    private readonly OrganizationManager organizationManager;

    public FinancialModel(OrganizationManager organizationManager)
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
        public string Name { get; set; } = default!;

        [Display(Name = "Taxpayer ID")]
        public string TaxpayerId { get; set; } = default!;

        [Display(Name = "Address")]
        public string Address { get; set; } = default!;

        [Display(Name = "Contact")]
        public string Contact { get; set; } = default!;

        [Display(Name = "Bank")]
        public string Bank { get; set; } = default!;

        [Display(Name = "Account")]
        public string Account { get; set; } = default!;
    }

}
