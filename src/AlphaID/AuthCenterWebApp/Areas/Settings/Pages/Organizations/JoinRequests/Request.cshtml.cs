using AlphaIdPlatform.JoinOrgRequesting;
using AlphaIdPlatform.Security;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Organizations.JoinRequests
{
    public class RequestModel(JoinOrganizationManager joinOrganizationManager, OrganizationManager organizationManager) : PageModel
    {
        [BindProperty]
        [Display(Name = "Organization name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string OrganizationName { get; set; } = null!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var org = await organizationManager.FindByNameAsync(OrganizationName);
            if (org == null)
                ModelState.AddModelError(nameof(OrganizationName), "Organization not found.");

            if (!ModelState.IsValid)
                return Page();

            var request = new JoinOrganizationRequest(User.SubjectId()!, org!.Name, org.Id);
            await joinOrganizationManager.Create(request);

            return RedirectToPage("Index");
        }
    }
}
