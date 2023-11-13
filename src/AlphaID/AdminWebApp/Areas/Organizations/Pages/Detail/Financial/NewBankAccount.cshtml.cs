using IDSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Financial
{
    public class NewBankAccountModel : PageModel
    {
        private OrganizationManager organizationManager;
        OrganizationBankAccountManager bankAccountManager;

        public NewBankAccountModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager)
        {
            this.organizationManager = organizationManager;
            this.bankAccountManager = bankAccountManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var org = await this.organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var org = await this.organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await this.bankAccountManager.AddAsync(org, this.Input.AccountNumber, this.Input.AccountName,
                this.Input.BankName, this.Input.Usage, this.Input.SetDefault);

            if (!this.Result.Succeeded)
                return this.Page();

            return this.RedirectToPage("Index", new { anchor });
        }

        public class InputModel
        {
            public string AccountNumber { get; set; } = default!;

            public string AccountName { get; set; } = default!;

            public string BankName { get; set; } = default!;

            public string? Usage { get; set; }

            public bool SetDefault { get; set; } = false;
        }
    }
}
