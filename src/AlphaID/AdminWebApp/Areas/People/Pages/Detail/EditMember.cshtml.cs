using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class EditMemberModel : PageModel
    {
        private readonly OrganizationMemberManager memberManager;
        private readonly NaturalPersonManager naturalPersonManager;

        public EditMemberModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager)
        {
            this.memberManager = memberManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGet(string id, string orgId)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(id);
            if (person == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersOfAsync(person);
            var member = members.FirstOrDefault(p => p.OrganizationId == orgId);
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

        public async Task<IActionResult> OnPostAsync(string id, string orgId)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(id);
            if (person == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersOfAsync(person);
            var member = members.FirstOrDefault(p => p.OrganizationId == orgId);
            if (member == null)
                return this.NotFound();

            member.Title = this.Input.Title;
            member.Department = this.Input.Department;
            member.Remark = this.Input.Remark;

            var result = await this.memberManager.UpdateAsync(member);
            if (result.IsSuccess)
            {
                return this.RedirectToPage("MembersOf", new { id });
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
