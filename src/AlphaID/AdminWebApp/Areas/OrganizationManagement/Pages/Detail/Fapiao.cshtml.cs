using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class FapiaoModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? organization = await organizationManager.FindByIdAsync(anchor);
        if (organization == null)
            return NotFound();

        if (organization.Fapiao != null)
            Input = new InputModel
            {
                Name = organization.Fapiao.Name,
                TaxpayerId = organization.Fapiao.TaxPayerId,
                Address = organization.Fapiao.Address,
                Contact = organization.Fapiao.Contact,
                Bank = organization.Fapiao.Bank,
                Account = organization.Fapiao.Account
            };

        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync(string anchor)
    {
        Organization? organization = await organizationManager.FindByIdAsync(anchor);
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        if (organization.Fapiao == null)
        {
            organization.Fapiao = new FapiaoInfo
            {
                Name = Input.Name,
                TaxPayerId = Input.TaxpayerId,
                Address = Input.Address,
                Contact = Input.Contact,
                Bank = Input.Bank,
                Account = Input.Account
            };
        }
        else
        {
            organization.Fapiao.Name = Input.Name;
            organization.Fapiao.TaxPayerId = Input.TaxpayerId;
            organization.Fapiao.Address = Input.Address;
            organization.Fapiao.Bank = Input.Bank;
            organization.Fapiao.Account = Input.Account;
        }

        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }

    public async Task<IActionResult> OnPostClearAsync(string anchor)
    {
        Organization? organization = await organizationManager.FindByIdAsync(anchor);
        if (organization == null)
            return NotFound();

        organization.Fapiao = null;
        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Name { get; set; } = null!;

        [Display(Name = "Taxpayer ID")]
        [Required(ErrorMessage = "Validate_Required")]
        public string TaxpayerId { get; set; } = null!;

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Address { get; set; } = null!;

        [Display(Name = "Contact")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Contact { get; set; } = null!;

        [Display(Name = "Bank")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Bank { get; set; } = null!;

        [Display(Name = "Account")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Account { get; set; } = null!;
    }
}