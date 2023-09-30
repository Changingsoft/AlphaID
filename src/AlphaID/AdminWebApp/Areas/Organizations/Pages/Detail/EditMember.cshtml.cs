using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail
{
    public class EditMemberModel : PageModel
    {
        private readonly OrganizationMemberManager memberManager;
        private readonly NaturalPersonManager naturalPersonManager;
        private readonly OrganizationManager organizationManager;

        public EditMemberModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager, OrganizationManager organizationManager)
        {
            this.memberManager = memberManager;
            this.naturalPersonManager = naturalPersonManager;
            this.organizationManager = organizationManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGet(string id, string personId)
        {
            var org = await this.organizationManager.FindByIdAsync(id);
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
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string personId)
        {
            var org = await this.organizationManager.FindByIdAsync(id);
            if (org == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersAsync(org);
            var member = members.FirstOrDefault(p => p.PersonId == personId);
            if (member == null)
                return this.NotFound();

            member.Title = this.Input.Title;
            member.Department = this.Input.Department;
            member.Remark = this.Input.Remark;

            var result = await this.memberManager.UpdateAsync(member);
            if (result.IsSuccess)
            {
                return this.RedirectToPage("Members", new { id });
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "职务")]
            [StringLength(50)]
            public string? Title { get; set; }

            [Display(Name = "部门")]
            [StringLength(50)]
            public string? Department { get; set; }

            [Display(Name = "备注")]
            [StringLength(50)]
            public string? Remark { get; set; }
        }
    }
}
