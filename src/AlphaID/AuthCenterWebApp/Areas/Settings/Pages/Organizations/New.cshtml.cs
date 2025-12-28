using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Transactions;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations;

public class NewModel(
    OrganizationManager organizationManager, IOrganizationStore store,
    UserManager<NaturalPerson> personManager) : PageModel
{
    [BindProperty]
    [Display(Name = "Name", Description = "Full name of organization.")]
    [Required(ErrorMessage = "Validate_Required")]
    [PageRemote(AdditionalFields = "__RequestVerificationToken", PageHandler = "CheckName", HttpMethod = "Post",
        ErrorMessage = "Organization name exists.")]
    public string Name { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Name = Name.Trim().Trim('\r', '\n').Replace(" ", string.Empty);
        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        if (store.Organizations.Any(p => p.Name == Name))
            ModelState.AddModelError(nameof(Name), "Organization already exists.");

        if (!ModelState.IsValid)
            return Page();

        var organization = new Organizational.Organization(Name)
        {
            Domicile = Input.Domicile,
            Representative = Input.Representative
        };

        OrganizationOperationResult result = await organizationManager.CreateAsync(organization);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.Errors.Aggregate((x, y) => $"{x}, {y}"));
            return Page();
        }

        //Add current person as owner.
        NaturalPerson? person = await personManager.GetUserAsync(User);
        Debug.Assert(person != null);

        try
        {
            var m = new OrganizationMember(person.Id, MembershipVisibility.Private)
            {
                Title = Input.Title,
                Department = Input.Department,
                Remark = Input.Remark,
                IsOwner = true
            };
            organization.Members.Add(m);
            var joinResult = await store.UpdateAsync(organization);
            if (!joinResult.Succeeded)
            {
                ModelState.AddModelError("", joinResult.Errors.Aggregate((x, y) => $"{x}, {y}"));
                return Page();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }

        trans.Complete();

        return RedirectToPage("Index");
    }

    public IActionResult OnPostCheckName(string name)
    {
        return store.Organizations.Any(o => o.Name == name)
            ? new JsonResult("Organization name exists.")
            : new JsonResult(true);
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