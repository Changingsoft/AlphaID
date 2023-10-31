using AlphaIDPlatform.Helpers;
using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail.Membership
{
    public class OfModel : PageModel
    {
        private readonly OrganizationMemberManager memberManager;
        private readonly NaturalPersonManager naturalPersonManager;

        public OfModel(OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager)
        {
            this.memberManager = memberManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        public OrganizationMember Member { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? OperationResult { get; set; }

        public IEnumerable<SelectListItem> MembershipVisibilties { get; set; } = EnumHelper.GetSelectListItems<MembershipVisibility>();

        public async Task<IActionResult> OnGet(string anchor, string orgId)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersOfAsync(person);
            var member = members.FirstOrDefault(p => p.OrganizationId == orgId);
            if (member == null)
                return this.NotFound();
            this.Member = member;
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

        public async Task<IActionResult> OnPostAsync(string anchor, string orgId)
        {
            var person = await this.naturalPersonManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            var members = await this.memberManager.GetMembersOfAsync(person);
            var member = members.FirstOrDefault(p => p.OrganizationId == orgId);
            if (member == null)
                return this.NotFound();
            this.Member = member;

            member.Title = this.Input.Title;
            member.Department = this.Input.Department;
            member.Remark = this.Input.Remark;
            member.IsOwner = this.Input.IsOwner;
            member.Visibility = this.Input.Visibility;

            this.OperationResult = await this.memberManager.UpdateAsync(member);

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

            [Display(Name = "Is owner", Description = "The owner of organization can fully mange organization by themselvs.")]
            public bool IsOwner { get; set; }

            [Display(Name = "Membership visibility")]
            public MembershipVisibility Visibility { get; set; }
        }
    }
}
