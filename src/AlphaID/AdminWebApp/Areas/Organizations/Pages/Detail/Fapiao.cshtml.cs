using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail
{
    public class FapiaoModel : PageModel
    {
        private readonly OrganizationManager organizationManager;

        public FapiaoModel(OrganizationManager organizationManager)
        {
            this.organizationManager = organizationManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var organization = await this.organizationManager.FindByIdAsync(anchor);
            if (organization == null)
                return this.NotFound();

            if (organization.Fapiao != null)
            {
                this.Input = new InputModel()
                {
                    Name = organization.Fapiao.Name,
                    TaxpayerId = organization.Fapiao.TaxPayerId,
                    Address = organization.Fapiao.Address,
                    Contact = organization.Fapiao.Contact,
                    Bank = organization.Fapiao.Bank,
                    Account = organization.Fapiao.Account,
                };
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostSaveAsync(string anchor)
        {
            var organization = await this.organizationManager.FindByIdAsync(anchor);
            if (organization == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            if (organization.Fapiao == null)
            {
                organization.Fapiao = new FapiaoInfo()
                {
                    Name = this.Input.Name,
                    TaxPayerId = this.Input.TaxpayerId,
                    Address = this.Input.Address,
                    Contact = this.Input.Contact,
                    Bank = this.Input.Bank,
                    Account = this.Input.Account,
                };
            }
            else
            {
                organization.Fapiao.Name = this.Input.Name;
                organization.Fapiao.TaxPayerId = this.Input.TaxpayerId;
                organization.Fapiao.Address = this.Input.Address;
                organization.Fapiao.Bank = this.Input.Bank;
                organization.Fapiao.Account = this.Input.Account;
            }

            this.Result = await this.organizationManager.UpdateAsync(organization);
            return this.Page();
        }

        public async Task<IActionResult> OnPostClearAsync(string anchor)
        {
            var organization = await this.organizationManager.FindByIdAsync(anchor);
            if (organization == null)
                return this.NotFound();

            organization.Fapiao = null;
            this.Result = await this.organizationManager.UpdateAsync(organization);
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
}
