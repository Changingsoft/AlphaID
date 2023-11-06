using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Members
{
    public class EditModel : PageModel
    {
        private readonly OrganizationMemberManager memberManager;
        private readonly OrganizationManager organizationManager;

        public EditModel(OrganizationMemberManager memberManager, OrganizationManager organizationManager)
        {
            this.memberManager = memberManager;
            this.organizationManager = organizationManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGet(string anchor, string personId)
        {
            var org = await this.organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersAsync(org);
            var member = members.FirstOrDefault(p => p.PersonId == personId);
            if (member == null)
                return this.NotFound();

            this.Input = new InputModel
            {
                Title = member.Title,
                Department = member.Department,
                Remark = member.Remark,
                IsOwner = member.IsOwner,
                Visibility = member.Visibility,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor, string personId)
        {
            var org = await this.organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersAsync(org);
            var member = members.FirstOrDefault(p => p.PersonId == personId);
            if (member == null)
                return this.NotFound();

            member.Title = this.Input.Title;
            member.Department = this.Input.Department;
            member.Remark = this.Input.Remark;
            member.IsOwner = this.Input.IsOwner;
            member.Visibility = this.Input.Visibility;

            var result = await this.memberManager.UpdateAsync(member);
            if (result.Succeeded)
            {
                return this.RedirectToPage("Index", new { anchor });
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Title")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Title { get; init; }

            [Display(Name = "Department")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Department { get; init; }

            [Display(Name = "Remark")]
            [StringLength(50, ErrorMessage = "Validate_StringLength")]
            public string? Remark { get; init; }

            public bool IsOwner { get; init; }

            public MembershipVisibility Visibility { get; init; } = MembershipVisibility.Private;
        }
    }
}
