using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Transactions;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations
{
    public class NewModel(OrganizationManager organizationManager, OrganizationMemberManager memberManager, NaturalPersonManager personManager) : PageModel
    {
        [BindProperty]
        [Display(Name = "Name")]
        [PageRemote(AdditionalFields = "__RequestVerificationToken", PageHandler = "CheckName", HttpMethod = "Post", ErrorMessage = "Organization name exists.")]
        public string Name { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Name = Name.Trim().Trim('\r', '\n').Replace(" ", string.Empty);
            using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (organizationManager.Organizations.Any(p => p.Name == Name))
                ModelState.AddModelError(nameof(Name), "Organization already exists.");

            if (!ModelState.IsValid)
                return Page();

            var organization = new GenericOrganization(Name)
            {
                Domicile = Input.Domicile,
                Representative = Input.Representative,
            };

            var result = await organizationManager.CreateAsync(organization);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.Aggregate((x, y) => $"{x}, {y}"));
                return Page();
            }
            //Add current person as owner.
            var person = await personManager.GetUserAsync(User);
            Debug.Assert(person != null);

            var member = new OrganizationMember(organization, person)
            {
                Title = Input.Title,
                Department = Input.Department,
                Remark = Input.Remark,
                IsOwner = true,
            };
            var joinOrgResult = await memberManager.CreateAsync(member);
            if (!joinOrgResult.Succeeded)
            {
                ModelState.AddModelError("", joinOrgResult.Errors.Aggregate((x, y) => $"{x}, {y}"));
                return Page();
            }
            trans.Complete();

            return RedirectToPage("Index");
        }

        public IActionResult OnPostCheckName(string name)
        {
            return organizationManager.FindByName(name).Any() ? new JsonResult("Organization name exists.") : new JsonResult(true);
        }

        public class InputModel
        {
            [Display(Name = "Domicile")]
            [StringLength(100, ErrorMessage = "Validate_StringLength")]
            public string? Domicile { get; set; }

            [Display(Name = "Representative")]
            [StringLength(20, ErrorMessage = "Validate_StringLength")]
            public string? Representative { get; set; }

            [Display(Name = "Title")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Title { get; set; }

            [Display(Name = "Department")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Department { get; set; }

            [Display(Name = "Remark")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Remark { get; set; }
        }
    }
}
