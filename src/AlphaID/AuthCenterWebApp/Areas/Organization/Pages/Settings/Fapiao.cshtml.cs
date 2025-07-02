using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings;

public class FapiaoModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
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
        var organization = await organizationManager.FindByNameAsync(anchor);
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
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        organization.Fapiao = null;
        Result = await organizationManager.UpdateAsync(organization);
        Input = null!;
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Taxpayer ID")]
        public string TaxpayerId { get; set; } = null!;

        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Display(Name = "Contact")]
        public string Contact { get; set; } = null!;

        [Display(Name = "Bank")]
        public string Bank { get; set; } = null!;

        [Display(Name = "Account")]
        public string Account { get; set; } = null!;
    }
}