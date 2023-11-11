using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Transactions;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations
{
    public class NewModel : PageModel
    {
        private readonly OrganizationManager organizationManager;
        private readonly OrganizationMemberManager memberManager;
        private readonly NaturalPersonManager personManager;

        public NewModel(OrganizationManager organizationManager, OrganizationMemberManager memberManager, NaturalPersonManager personManager)
        {
            this.organizationManager = organizationManager;
            this.memberManager = memberManager;
            this.personManager = personManager;
        }

        [BindProperty]
        [Display(Name = "Name")]
        [PageRemote(AdditionalFields = "__RequestVerificationToken", PageHandler = "CheckName", HttpMethod = "Post", ErrorMessage = "Organization name exists.")]
        public string Name { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Unified social credit code\r\n")]
        public string? Usci { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet()
        {
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.Name = this.Name.Trim().Trim('\r', '\n').Replace(" ", string.Empty);
            using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (this.organizationManager.Organizations.Any(p => p.Name == this.Name))
                this.ModelState.AddModelError(nameof(this.Name), "Organization already exists.");
            UnifiedSocialCreditCode usci = new();
            if (this.Usci != null)
            {
                if (UnifiedSocialCreditCode.TryParse(this.Usci, out usci))
                {
                    if (this.organizationManager.Organizations.Any(p => p.Usci == usci.ToString()))
                        this.ModelState.AddModelError(nameof(this.Usci), "USCI already exists");
                }
                else
                    this.ModelState.AddModelError(nameof(this.Usci), "Invalid USCI");
            }

            if (!this.ModelState.IsValid)
                return this.Page();

            var organization = new GenericOrganization(this.Name)
            {
                Usci = this.Usci != null ? usci.ToString() : null,
                Domicile = this.Input.Domicile,
                Representative = this.Input.Representative,
            };

            var result = await this.organizationManager.CreateAsync(organization);
            if (!result.Succeeded)
            {
                this.ModelState.AddModelError("", result.Errors.Aggregate((x, y) => $"{x}, {y}"));
                return this.Page();
            }
            //Add current person as owner.
            var person = await this.personManager.GetUserAsync(this.User);
            Debug.Assert(person != null);

            var member = new OrganizationMember(organization, person)
            {
                Title = this.Input.Title,
                Department = this.Input.Department,
                Remark = this.Input.Remark,
                IsOwner = true,
            };
            var joinOrgResult = await this.memberManager.CreateAsync(member);
            if (!joinOrgResult.Succeeded)
            {
                this.ModelState.AddModelError("", joinOrgResult.Errors.Aggregate((x, y) => $"{x}, {y}"));
                return this.Page();
            }
            trans.Complete();

            return this.RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostCheckName(string name)
        {
            if (this.organizationManager.SearchByName(name).Any())
                return new JsonResult("Organization name exists.");
            return new JsonResult(true);
        }

        public class InputModel
        {
            [Display(Name = "Domicile")]
            [StringLength(100)]
            public string? Domicile { get; init; }

            [Display(Name = "Representative")]
            [StringLength(20)]
            public string? Representative { get; init; }

            [Display(Name = "Title")]
            [StringLength(50)]
            public string? Title { get; init; }

            [Display(Name = "Department")]
            [StringLength(50)]
            public string? Department { get; init; }

            [Display(Name = "Remark")]
            [StringLength(50)]
            public string? Remark { get; init; }
        }
    }
}
